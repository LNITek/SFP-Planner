# How To Create Your Own Mods.

## Power Gen Recipes :
* ID Field Must Have The Mod Tag (M_).
* Recipes Can Get Confusing Very Fast.
* There Is No Need To Add A Primary Output Item.
* The Power Output Is The Save As The `PowerUsed` In The `BuildingsDB`
* No Spaces For Input And Output Columns.
* To Use Build In Items And Buildings ID's, Don't Add The Mod Tag (M_).
### File Name
PowerGenDB.csv
### File Structure
|     | ID  | Name | Factory | Input | Output |
| --- | --- | ---- | ------- | ----- | ------ |
| **Structure** | M_ | RecipeName | Building ID | Input ID <code>&#124;</code> Per Min | Output ID <code>&#124;</code> Per Min|
| **Example 1** | M1 | Coal | 15 | 5<code>&#124;</code>15 |  |
| **Example 2** | M2 | Uranium | 18 | 78<code>&#124;</code>0.2 | 83<code>&#124;</code>10 |
| **Example 3** | M3 | Hydro Plant | M1 | 34<code>&#124;</code>100 |  |
| **Example 4** | M3 | RTG | M3 | M1<code>&#124;</code>1 |  |

### Structure Example
```
ID,Name,Factory,Input,Output
M1,Coal,15,5|15,
M2,Uranium,18,78|0.2,83|10
M3,Hydro Plant,M1,34|100,
M3,RTG,M3,M1|1,
```