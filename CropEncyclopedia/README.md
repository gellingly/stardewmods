# Crop Encyclopedia

Press a configurable keybinding to open a UI that provides crop information like sell price, gold per day, and what seasons it can be grown in.

The columns, in order, are:
* Name
* Sprite
* D - How many days it takes to grow to first harvest
* R - How many days it takes to regrow, if applicable
* \# - Number of harvests you'd be able to make from one seed if planted today.  This does the calculation for a maximum of one year if greenhouse logic is being used, or the crop regrows and can last through all seasons.
* G - Sell price
* G/d - Gold per day.  If the crop does not regrow, this is `sell price / days for growth`.  If the crop regrows, it is `sell price * number of harvests / days until last harvest`.  This does not take into account crops that might have extra yield, such as blueberries.  Similar to the number of harvests, this does the calculation for a maximum of one year if greenhouse logic is being used, or the crop regrows and can last through all seasons.
* Date - Date of first harvest if planted today
* Seasons - What seasons this crop is in season for

All column headers can be clicked to sort the information by that column, except for Seasons.

The options on the left, in order,  are:
* Textbox - Filter by name
* Greenhouse Logic - Changes values such as the filter for if the crop will die before being harvestable, how many days until harvest, first harvest date, and gold per day to be calculated assuming the crop is always in season
* Won't die - Only show crops that will be harvestable if planted today
* Giant crop - Only show crops that can form a giant crop
* Seasons - Filter by season
* Regrowth - Filter by if the crop can regrow
* Type - Filter by type like vegetable, fruit, or flower
* Fertilizer - Changes values such as days until harvest, first harvest date, and gold per day to reflect the chosen fertilizer

If the farmer has the Agriculturalist profession, calculations will take this into account.

## Config Options
KeyBinding - keybinding to open the UI.  Default is F8.
