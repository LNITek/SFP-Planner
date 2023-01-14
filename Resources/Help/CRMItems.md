# How To Create Your Own Mods.

## Items :
* ID Field Must Have The Mod Tag (M_).
### File Name
ItemsDB.csv
### File Structure
|     | ID  | Name | Category |
| --- | --- | ---- | -------- |
| **Structure** | M_  | ItemName | Category |
| **Example 1** | M1  | FICSMAS Gift | FICSMAS |
| **Example 2** | M20 | Box | Other |

### Structure Example
```
ID,Name,Category
M1,FICSMAS Gift,FICSMAS
M2,Candy Cane,FICSMAS
M3,Actual Snow,FICSMAS
```

### Valid Categories
```
Ores,
Ingots,
Material,
Liquids,
Gas,
StdParts,
IndParts,
AdvParts,
Nuclear,
Consumed,
Container,
Special,
FICSMAS,
Other
```