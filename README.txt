The patterns we implemented in this project were the Command, Strategy, State, Mediator, and Observer patterns.
The Command, Strategy, and State patterns are all labeled, however, the Mediator pattern is the main file of PlayerMovement
and the Observer pattern is in the form of the observer class being event as this type allows other objects to subscribe to it
Which is shown in MoveCamera in Start and HandleSlideStart and HandleSlideStop being the "update" functions for these observers.

You can find the code by going to assets/scripts and assets/tests for the test code.

In order to test the game if you don't have unity editor go to the build folder and run the .exe / application called
Group-project. I am unsure if this will work on non Windows OS so proceed with that in mind.

In the game there are features of sliding on left ctrl, moving with wasd, jump and walljump with spacebar, and dashing with
shift. Along with coyote time and jump buffer being enabled for all of these techniques.