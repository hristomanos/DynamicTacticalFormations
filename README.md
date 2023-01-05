# Tactical battle formations

# Summary

* Made with Unity and C# in 2 months 
* 3D Real-time strategy game environment setup with camera controls and marquee selection
* Units can be arranged in several battle formations algorithmically
* A virtual leader object is responsible for handling formations
* Units in formation move in coordination to a user-defined destination

# Introduction
<p align="Justify">
Formations add tactical depth to offensive and defensive decision making when players micromanage organised groups in strategy games. This paper investigates the history of tactical formations and their use in video games. It implements a coordinated movement system based on the concepts of virtual leaders and predefined fixed slots. This approach is tested in a Unity 3D game environment using C#. From the results, it concludes that navigating around the environment as a group can be challenging but work around solutions can be applied to specific cases.
</p>

## Demo video ##
<p align="Centre">
[![DemoImage](http://img.youtube.com/vi/ubPr9XzjK8Q/0.jpg)](https://youtu.be/ubPr9XzjK8Q)
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

<img src="https://github.com/hristomanos/DynamicTacticalFormations/blob/main/SquareFormation.png" width="300.75" height="441" />

#### Wedge
<p align="Justify">
Similarly, to a square formation, the wedge formation is calculated from a grid. It starts from placing the leader alone at the top as its origin point. Then, to gradually add more units at each row to create a triangle shape, the number of columns in each row is increased by one after each row iteration. Next, the offset starts at 0 for the leader and decreases the spacing between units by half the more units are added in each row.  The X value is calculated by the product of the current column index and the specified spacing minus the offset. Lastly, the Z value equals to the product of the current row and the spacing between units.
</p>
  
![LineFormation](https://github.com/hristomanos/DynamicTacticalFormations/blob/main/WedgeFormation.png)
  
#### Line
<p align="Justify">
A single line of units can be calculated by multiplying the index of each unit by the horizontal spacing. Since the virtual leader is placed in the middle of the formation, an offset is subtracted from the multiplication to scatter the positions uniformly across the line. The offset value is applied to all formations and is calculated by multiplying the number of units by the horizontal spacing divided by two to achieve even distribution across the line.
</p>

![LineCode](https://github.com/hristomanos/DynamicTacticalFormations/blob/main/LineCode.png)
![LineFormation](https://github.com/hristomanos/DynamicTacticalFormations/blob/main/LineFormation.png)


