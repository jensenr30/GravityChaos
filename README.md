![gravity chaos banner](pics/banner.png?raw=true "Gravity Chaos")


# GravityChaos
Simulation of gravitational attraction, and motion of particles in 2D space.
The purpose is to generate satisfying images.


## How it Works

I found the concept for this program in this video: [Chaos, by Rudy Rucker](https://youtu.be/ICrNOTQBS8U?t=217).  Between 3:37 and 5:00, it provides a concise visual explanation of how these images are generated.

A number of particles are created and kept stationary; these are the targets.

One particle is created that is mobile; this is the projectile.

The projectile is placed at a certain pixel on the screen, `<x,y>`. The projectile experiences a force exerted upon it by the stationary targets. This force is calculated to mimic gravity, so it follows the [inverse-square law](https://en.wikipedia.org/wiki/Inverse-square_law). The projectile is accelerated and travels in 2D space. If it hits one of the targets during its travels, the initial starting point (the pixel at `<x,y>`) is set to the color of the target.

Therefore, the color of each pixel on the image indicates which target the projectile will eventually hit.

If a pixel is black, that indicates the simulation terminated before the projectile hit any target.

Each target is rendered as a large, color-filled circle.


## Chaotic Regions

In some areas of the image, small changes in the starting position of the projectile can make it land on different targets. Because of this sensitivity to initial conditions, the image tends to look chaotic in some regions. An example of a chaotic region is the following:

![chaotic region example](pics/chaotic%20region%20example.png?raw=true "A Chaotic Region")

In other regions of the map, the terminal location of the projectile is not very sensitive to initial conditions. In these areas, the image does not look chaotic; rather, it appears simple and well-behaved.


## User Interface
Currently, no user interface exists. As this is just an experimental program, all the particles are specified [in the source code](GravityChaos/GravityChaos/Form1.cs).


## Image Gallery
Click on images to view them at full-resolution in a new tab.

![Granular Lava Lamp](pics/granular%20lava%20lamp%202%20high%20res.png?raw=true "Granular Lava Lamp")

![Ornate Linear Array](pics/ornate%20linear%20array.png?raw=true "Ornate Linear Array")

![Matrix Gradient](pics/matrix%20gradient%20high%20res.png?raw=true "Matrix Gradient")

![Hexagonal Grayscale](pics/hexagonal%20grayscale.png?raw=true "Hexagonal Grayscale")

![snow drift](pics/snow%20drift%20high%20res.png?raw=true "Snow Drift")

More images can be found in the [pics](pics/) folder in this repository.


## Videos
Here are some animations that were created by generating a sequence of images, and using ffmpeg to compile them into a video.

Click on any of the pictures below to open the video.

[![Gelatinous Alien Clockwork](https://img.youtube.com/vi/q59parajCWQ/0.jpg)](https://www.youtube.com/watch?v=q59parajCWQ)

[![Chameleon](https://img.youtube.com/vi/00W9rgBazVI/0.jpg)](https://www.youtube.com/watch?v=00W9rgBazVI)
