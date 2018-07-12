![gravity chaos banner](pics/banner.png "Gravity Chaos")

# GravityChaos
Simulation of gravitational attraction, and motion of particles in 2D space.
The purpose is to generate satisfying images.

## How it Works

A number of particles are created and kept stationary; these are the `targets`.

One particle is created that is mobile; this is the `projectile`.

The `projectile` is placed at a certain pixel on the screen, `<x,y>`. The `projectile` has a force exerted upon it by the stationary `targets`. The code that calculates this force was written to simulate gravity, so it follows the [inverse-square-law](https://en.wikipedia.org/wiki/Inverse-square_law). The `projectile` is accelerated and travels in 2D space. If it hits one of the `targets`, the starting point pixel at `<x,y>` is set to the color of the `target`.

Therefore, the color of each pixel at on the map shows you which `target` the `projectile` will hit when the simulation starts with the `projectile` on that pixel.

If a pixel is black, that indicates the simulation terminated before the projectile hit any target. As the simulation is allowed to run for more iterations before terminating, the images produced will be more and more dense with colored (non-black) pixels. This is because  the `projectile` is given more and more opportunities to collide with a `target`.

## Chaotic Regions

At some points on the map, small changes in initial conditions will produce wildly different outcomes! A minor change in starting position of the `projectile` can easily make it land on `target 1` instead of `target 2`. Because of this sensitivity to initial conditions, the map tends to look chaotic in some regions. An example of a chaotic region is the following:

![chaotic region example](

In other regions of the map, its character tends to be very well behaved. This happens when the destination of the `projectile` is not sensitive to initial conditions.

## Image Gallery
Here are some of the more pleasing/interesting results from this particle/gravity simulation program:

![Granular Lava Lamp](pics/granular%20lava%20lamp%202%20high%20res.png "Granular Lava Lamp")
