# SpatialPartitionUnity
Spatial partition is used to optimize games by storing each game entity based on its location within a grid. Spatial partition lets you query objects that are near. This pattern is mentioned in the book <a href="https://gameprogrammingpatterns.com/"> Game Programming Patterns by Robert Nystrom</a>.

# Why should you use it ? 
Because using Unity's physics components in order to calculate collisions are costly. You can use this pattern in order to spawn more objects and implement their logic.

# About This Project 
Most of the implementations of this pattern in Unity does not work. Also they are unnecessary complex.
You can use this template to understand how Spatial Partition works.
This project includes two scenes . "SpatialPartititon" scene is the one that implements this pattern and "TestCase" scene is the one that works default physics components rather than the spatial partition.
Player has ability to spawn bullets that destroy enemies on impact. Each bullet spawns other 8 bullets if they interact with an enemy (This makes the "TestScene" crash because that creates lots of enemies). Also each bullet has it's lifespan. While multiplying , new bullet inherits the lifetime of the old bullet.


# Performance Metrics
Before the FPS is down below 30 , 12000 Enemy entities are spawned on the "Spatial Partition" scene and 800 entities are spawned on "Test Case" scene.
Overall performance is improved by %1500 without using any multithreading library or framework. 
