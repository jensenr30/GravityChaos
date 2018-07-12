# GravityChaos
Simulation of gravitational attraction, and motion of particles in 2D space.
The purpose is to generate satisfying images and satisfy some curiosity.

## How it Works

A number of particles are created and fixed in place; these are the `targets`.

One particle is created that is mobile; this is the `projectile`.

The `projectile` is placed at a certain point on the screen, `<x,y>`. The `projectile` has a force exerted upon it by the stationary `targets`. This force follows the [inverse-square-law](https://en.wikipedia.org/wiki/Inverse-square_law) like gravity. The `projectile` is accelerated and travels in 2D space. If it hits one of the `targets`, the starting point pixel at `<x,y>` is set to the color of the `target`.

At some points on the map, small changes in initial conditions will produce wildy different outcomes! A minor change in starting position of the `projectile` can easily make it land on `target 1` instead of `target 2`. Because of this sensitivity to initial conditions, the map tends to look chaotic in some regions.

In other regions of the map, its character tends to be very well behaved. This happens when the destination of the `projectile` is not sensitive to initial conditions.

## Image Gallery
Here are some of the more pleasing/interesting results from this particle/gravity simulation program:

![at ext](https://github.com/jensenr30/GravityChaos/blob/master/pics/granular%20lava%20lamp%202%20high%20res.png?raw=true "Granular Lava Lamp")
