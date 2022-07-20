# Simple2DGame
Making a 2-D platformer following Brackeys' Unity Tutorial - https://www.youtube.com/playlist?list=PLPV2KyIb3jR42oVBU6K2DIL6Y22Ry9J1c

I wanted to practice developing movement controls for a player character in a 2D platformer in preparation for a future project.

## Objectives 
I wanted to get some experience figuring out and fixing various problems w/ regards to 2D movement.
Among these include:
* **Crouching** - Deal with crouch hitboxes and crouch hitboxes interacting w/ environment(ceilingcheck).
* **Jumping** - Figure out best(my preferred) way to handle Y-axis movement (apply force vs overwrite velocity vs translate).
* **Slopes** - Still not super satisfied but got to work with Circlecasts and RayCasts. Ended up making player frictionless on slopes to go up.
* **Update vs FixedUpdate and state machines** - Developed a workflow for updating the player state and then using FixedUpdate to update the game state.
* **Animations** - Set up various animation states and triggers(conditions) for each animation so that the character sprite reflects player state.
* **OnValidate** - Developed workflow for automatically finding components in Inspector via OnValidate workflow, instead of just using GetComponent everywhere.
* **Vectors** - Practice using and manipulating Vectors to determine various game state interactions




