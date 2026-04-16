param(
    [Parameter(Mandatory=$true)]
    [string]$ApiKey,
    
    [Parameter(Mandatory=$false)]
    [string]$CalorieFile = "CommonCalories.md"
)

# USDA FoodData Central API endpoint
$apiBase = "https://api.nal.usda.gov/fdc/v1"

function Search-Food {
    param(
        [string]$FoodName,
        [string]$ApiKey
    )
    
    $searchUrl = "$apiBase/foods/search?api_key=$ApiKey&query=$FoodName&pageSize=10"
    
    try {
        $response = Invoke-RestMethod -Uri $searchUrl -Method Get
        return $response.foods
    }
    catch {
        Write-Warning "Error searching for '$FoodName': $_"
        return $null
    }
}

function Get-CaloriesPer100g {
    param(
        [object]$Food
    )
    
    # Look for energy in kcal per 100g
    $energyNutrient = $Food.foodNutrients | Where-Object { 
        $_.nutrientName -like "*Energy*" -and $_.unitName -eq "KCAL"
    } | Select-Object -First 1
    
    if ($energyNutrient) {
        return [math]::Round($energyNutrient.value, 0)
    }
    
    return $null
}

function Show-FoodOptions {
    param(
        [array]$Foods,
        [string]$IngredientName
    )
    
    Write-Host "`nFound multiple options for '$IngredientName':" -ForegroundColor Cyan
    Write-Host ""
    
    for ($i = 0; $i -lt $Foods.Count; $i++) {
        $food = $Foods[$i]
        $calories = Get-CaloriesPer100g -Food $food
        $dataType = $food.dataType
        $brand = if ($food.brandOwner) { " [$($food.brandOwner)]" } else { "" }
        
        Write-Host "$($i + 1). $($food.description)$brand" -ForegroundColor Yellow
        Write-Host "   Type: $dataType | Calories: $calories kcal/100g" -ForegroundColor Gray
    }
    
    Write-Host "S. Skip this ingredient" -ForegroundColor Yellow
    Write-Host ""
    
    do {
        $choice = Read-Host "Select option (1-$($Foods.Count) or S to skip)"
        
        if ($choice -eq 'S' -or $choice -eq 's') {
            return $null
        }
        
        $choiceNum = $null
        if ([int]::TryParse($choice, [ref]$choiceNum)) {
            if ($choiceNum -ge 1 -and $choiceNum -le $Foods.Count) {
                return $Foods[$choiceNum - 1]
            }
        }
        
        Write-Host "Invalid choice. Please enter 1-$($Foods.Count) or S" -ForegroundColor Red
    } while ($true)
}

function Update-CalorieFile {
    param(
        [string]$FilePath,
        [string]$Ingredient,
        [int]$Calories,
        [decimal]$Ratio
    )
    
    $content = Get-Content $FilePath
    
    for ($i = 0; $i -lt $content.Count; $i++) {
        $line = $content[$i]
        
        # Check if this line starts with the ingredient name
        if ($line -match "^$([regex]::Escape($Ingredient))\s*\|") {
            # Replace the line with updated values
            $content[$i] = "$Ingredient | $Calories | $Ratio"
            break
        }
        # Handle "Potato" line without pipes
        elseif ($line -eq $Ingredient) {
            $content[$i] = "$Ingredient | $Calories | $Ratio"
            break
        }
    }
    
    Set-Content -Path $FilePath -Value $content
}

# Main script
Write-Host "==================================================" -ForegroundColor Green
Write-Host "USDA FoodData Central - Calorie Lookup Tool" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host ""

# Read the current calorie file
if (-not (Test-Path $CalorieFile)) {
    Write-Error "File not found: $CalorieFile"
    exit 1
}

$content = Get-Content $CalorieFile

# Parse the markdown table to find missing entries
$missingIngredients = @()
$inTable = $false

foreach ($line in $content) {
    # Skip the header separator line
    if ($line -match '^---') {
        $inTable = $true
        continue
    }
    
    # Skip the header line
    if ($line -match '^Ingredient \|') {
        continue
    }
    
    # Match lines with ingredient name and empty calories/ratio
    if ($inTable -and $line -match '^([^|]+)\s*\|\s*\|') {
        $parts = $line -split '\|'
        if ($parts.Count -ge 3) {
            $ingredient = $parts[0].Trim()
            $calories = $parts[1].Trim()
            $ratio = $parts[2].Trim()
            
            # If ingredient has a name and is missing calories or ratio, add it
            if ($ingredient -and ($calories -eq '' -or $ratio -eq '')) {
                $missingIngredients += $ingredient
            }
        }
    }
    # Handle lines without pipes (like "Potato")
    elseif ($inTable -and $line -match '^\w+' -and $line -notmatch '\|') {
        $ingredient = $line.Trim()
        if ($ingredient) {
            $missingIngredients += $ingredient
        }
    }
}

if ($missingIngredients.Count -eq 0) {
    Write-Host "No missing calorie data found!" -ForegroundColor Green
    exit 0
}

Write-Host "Found $($missingIngredients.Count) ingredients with missing data:" -ForegroundColor Cyan
$missingIngredients | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
Write-Host ""

$updatedCount = 0

foreach ($ingredient in $missingIngredients) {
    Write-Host "`n----------------------------------------" -ForegroundColor Magenta
    Write-Host "Looking up: $ingredient" -ForegroundColor White
    Write-Host "----------------------------------------" -ForegroundColor Magenta
    
    # Clean up ingredient name for search
    $searchTerm = $ingredient -replace '\s*\([^)]*\)', ''  # Remove parenthetical text
    
    $foods = Search-Food -FoodName $searchTerm -ApiKey $ApiKey
    
    if (-not $foods -or $foods.Count -eq 0) {
        Write-Warning "No results found for '$ingredient'"
        continue
    }
    
    # Filter to foods with calorie data
    $foodsWithCalories = $foods | Where-Object {
        $calories = Get-CaloriesPer100g -Food $_
        $calories -ne $null
    }
    
    if ($foodsWithCalories.Count -eq 0) {
        Write-Warning "No foods with calorie data found for '$ingredient'"
        continue
    }
    
    # If multiple options, let user choose
    $selectedFood = if ($foodsWithCalories.Count -gt 1) {
        Show-FoodOptions -Foods $foodsWithCalories -IngredientName $ingredient
    } else {
        $foodsWithCalories[0]
    }
    
    if (-not $selectedFood) {
        Write-Host "Skipped: $ingredient" -ForegroundColor Yellow
        continue
    }
    
    $calories = Get-CaloriesPer100g -Food $selectedFood
    $ratio = [math]::Round($calories / 100, 2)
    
    Write-Host "`nSelected: $($selectedFood.description)" -ForegroundColor Green
    Write-Host "Calories/100g: $calories" -ForegroundColor Green
    Write-Host "Ratio: $ratio" -ForegroundColor Green
    
    # Update the file
    Update-CalorieFile -FilePath $CalorieFile -Ingredient $ingredient -Calories $calories -Ratio $ratio
    
    $updatedCount++
    Write-Host "Updated in file" -ForegroundColor Green
    
    # Small delay to avoid rate limiting
    Start-Sleep -Milliseconds 250
}

Write-Host "`n==================================================" -ForegroundColor Green
Write-Host "Complete! Updated $updatedCount ingredients" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
