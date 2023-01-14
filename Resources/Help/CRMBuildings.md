# How To Create Your Own Mods.

## Buildings :
* ID Field Must Have The Mod Tag (M_).
### File Name
BuildingsDB.csv
### File Structure
|     | ID  | Name | Category | PowerUsed |
| --- | --- | ---- | -------- | --------- |
| **Structure** | M_  | BildingName | Category | Max MW |
| **Example 1** | M1  | Smelter | Production | 4 |
| **Example 2** | M1  | Coal Generator | Generators | 75 |
| **Example 3** | M20 | Master Maker | Other | 100 |

### Structure Example
```
ID,Name,Category,PowerUsed
M1,Master Maker,Other,100
M2,Supper Furnace,Production,80
M3,Hidro Plant,Generator,100
```

### Valid Categories
```
Extraction,
Production,
Generators,
Special,
Other
```