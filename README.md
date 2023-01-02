# Battle formations

# Brief
<p align="Justify">
* Made with Unity and C# in 2 months 
* 3D Real-time strategy game environment settup with camera controls and marquee selection
* Units can be arranged in various battle formations algorithmically
* A virtual leader is 5
* Formation move in coordination
</p>

# Introduction
<p align="Justify">
Formations add tactical depth to offensive and defensive decision making when players micromanage organised groups in strategy games. This paper investigates the history of tactical formations and their use in video games. It implements a coordinated movement system based on the concepts of virtual leaders and predefined fixed slots. This approach is tested in a Unity 3D game environment using C#. From the results, it concludes that navigating around the environment as a group can be challenging but work around solutions can be applied to specific cases.
</p>

## Implementation

### Virtual leader
<p align="Justify">
The virtual leader is responsible for handling formations. He creates them, establishes a squad, and provides each member a relative position in the formation after each frame. Individual path planning is occurring within the formation to each unit that is trying to stay as close to their assigned positions as possible. During group movement, the leader decides on the path that the whole formation will take. For maintaining distance within the group, the leader will mitigate the squad’s speed based on how scattered the formation is. Units that are catching up will gain a temporary boost of speed and units already in the formation will slow down. 
</p>

### Formations

#### Square
<p align="Justify">
The square formation varies in shape based on the number of units in a row or in a column. The calculation is similar to generating a 2D grid. It is a nested loop that involves scanning the grid row by row. A for loop working on columns is placed within another for loop operating on rows. The number of units in a column depends on a user specified number and the number of units in a row depends on the total number of selected units.

  Like the line formation, the leader should be centred on the middle of the first row and the formation should distribute evenly on both sides in relation to the leader. Therefore, the X value comes from the multiplication of the current column index and the user specified spacing minus the offset. The Y value is similar to the X value but without the offset. 
</p>

#### Column

#### Line
#### 
