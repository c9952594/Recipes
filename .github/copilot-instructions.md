# Copilot Instructions for Recipe Collection

This is a personal recipe collection following the Grocery Recipe Format (https://github.com/cnstoll/Grocery-Recipe-Format), a markdown-based format designed for human readability and digital longevity.

## Project Structure & Organization

### Directory Architecture
- **Categorized by meal type**: `Breakfast/`, `Lunch/`, `Dinner/`, `Pudding/`
- **Reference files**: `CommonCalories.md` (ingredient calorie database), `USAConversions.md` (unit conversions)
- **Archive**: `Archived/` for recipes no longer in rotation
- **Uncategorized**: `Uncategorised/` for recipes not yet classified

### Recipe Format Standards

#### Two Recipe Formats Coexist:
1. **Structured Format** (preferred for new recipes) - see `Breakfast/Shakshuka.md`:
   ```markdown
   # Recipe Title
   
   Weight | Calories | Ratio
   --- | --- | ---
   [total weight] | [total calories] | [calories per gram]
   
   Portions | Weight | Calories
   --- | --- | ---
   [servings] | [weight per serving] | [calories per serving]
   
   ## Ingredients
   ### [Section Name]
   Ingredient | Amount | Calories | Preparation
   --- | --- | --- | ---
   ```

2. **Legacy Format** (from external sources) - see `Dinner/Chilli.md`:
   - Raw text format from recipe websites
   - Often includes source URL at top
   - No structured calorie tracking

#### Calorie Calculation System
- Uses `CommonCalories.md` as ingredient reference (calories per 100g with ratio multipliers)
- Each ingredient entry includes weight, calculated calories, and preparation notes
- Total recipe calories and per-serving breakdowns at top of structured recipes
- Example calculation: `Butter | 40g | 298` (using 7.45 ratio from CommonCalories.md)

#### Ingredient Organization Patterns
- Group ingredients by cooking stage: "Base", "Sauce", "Topping", "Side"
- **List ingredients in the order they are used** in the recipe instructions
- Use consistent measurement format: `180g (1)` shows both weight and count
- Preparation column for prep instructions: "Sliced", "Diced", "Crushed"

## Content Creation Guidelines

### When Adding New Recipes:
1. Use structured format for personal recipes
2. Calculate calories using `CommonCalories.md` ratios
3. Include total weight, calories, and per-serving breakdown
4. Organize ingredients by cooking stages
5. Keep instructions concise and stage-based
6. If the source page includes comments, customer reviews, suggestions, or tips, add a `Suggestions` section at the bottom of the recipe with a concise summary

### When Converting Legacy Recipes:
1. Preserve original source URL if external
2. Gradually convert to structured format when cooking
3. Add calorie calculations when ingredients are available in `CommonCalories.md`
4. Include a `Suggestions` section at the bottom when the source includes useful customer comments, review summaries, or variation tips

### Reference File Maintenance:
- Update `CommonCalories.md` when adding new ingredients
- Use format: `Ingredient | Calories/100g | Ratio` where ratio = calories/(total weight)g
- Update `USAConversions.md` for volume-to-weight conversions

## File Naming Conventions
- Use PascalCase: `CookieDoughOat.md`, `AllWeekSalad.md`
- No spaces in filenames
- Descriptive names that indicate dish type

## Special Files
- Markdown tables are core to the format - preserve alignment and structure