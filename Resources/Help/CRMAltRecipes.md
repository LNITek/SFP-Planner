# How To Create Your Own Mods.

## Alt Recipes :
* ID Field Must Have The Mod Tag (M_).
* Can Not Have Alt Recipes Without A Default Recipe.
* If You Want To Use Custom Items And Buildings In Your Recipes Then You Must Have A ItemsDB And Or BuildingDB File In Your Mod Folder
* Recipes Can Get Confusing Very Fast.
* The First Output Is Reserved  For The Primary Item For.
* No Spaces For Input And Output Columns.
* To Use Build In Items And Buildings ID's, Don't Add The Mod Tag (M_).
### File Name
AltResipesDB.csv
### File Structure
|     | ID  | Name | Factory | Input | Output |
| --- | --- | ---- | ------- | ----- | ------ |
| **Structure** | M_ | RecipeName | Building ID | Input ID <code>&#124;</code> Per Min | Output ID <code>&#124;</code> Per Min|
| **Example 1** | M1  | FICSMAS Gift | 0 |  | M1<code>&#124;</code>1|
| **Example 2** | M8 | Candy Cane | 7 | M1<code>&#124;</code>15 |M2<code>&#124;</code>5 |
| **Example 3** | M8 | Sweet Fireworks | 8 | M10<code>&#124;</code>15 ; M2<code>&#124;</code>7.5 |M14<code>&#124;</code>2.5 |
| **Example 4** | M1 | Supper Ingot | M1 | 14<code>&#124;</code>10 ; 15<code>&#124;</code>10 ; M1<code>&#124;</code>10 | M2<code>&#124;</code>10 ; 11 <code>&#124;</code>1|

### Structure Example
```
ID,Name,Factory,Input,Output
M1,FICSMAS Gift,0,,M1|1
M8,Candy Cane,7,M1|15,M2|5
M11,Sweet Fireworks,8,M10|15;M2|7.5,M14|2.5
M1,Supper Ingot,M1,14|15;15|7.5;M1|10,M2|10;11|1
```