# Unity_Flight_Sim
TMS flight sim in unity
<br> 
Initial Use:<br>
1  - Clone package into workspace.<br>
2  - Open Unity_Flight_Sim/Assests/Main.Unity in Unity 5.5<br>
3  - Click on 'Terrain_script' within Main's heierarchy<br>
4  - Edit the scripts input components from the Inspector panel<br>
5  - Field - Heightmap = absolute location to 16 bit gray pngs tile dir<br>
6  - Field - Grid size = the amount of tiles in a system, ie: 3 = 3x3 = 9 terrain tiles<br>
7  - Field - Terrain Size = the size of each terrain chunk in unity<br>
8  - Field - Height = Resolution Height for terrain (100 seems to be a good value for me)<br>
9  - Field - Offset = size of image should be a [power of 2] + 1 (we use 257 sicne this is our image size)<br>
10  - Field - Start_x = start location from tms structure, x location is our folder within tms/10/XX (use 205)<br>
11 - Field - Start_y = start location from tms structure, y location is our image in tms/10/XX/YY.png (use 640)<br>
<br>
Testing:<br>
Hit the play button then wait for terrain to be built.<br>
Hit play again after done testing.<br>
