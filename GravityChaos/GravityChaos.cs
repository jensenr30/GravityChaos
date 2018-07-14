using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace GravityChaos
{
    class Particle
    {
        // define parameters of the particle        // units
        public double PositionX     { get; set; }   // meters
        public double PositionY     { get; set; }   // meters
        public double VelocityX     { get; set; }   // meters / second
        public double VelocityY     { get; set; }   // meters / second
        public double ForceX        { get; set; }   // newtons (kg * m / s^2)
        public double ForceY        { get; set; }   // newtons (kg * m / s^2)
        public double Mass          { get; set; }   // kilograms
        public double Radius        { get; set; }   // meters
        public bool   Fixed         { get; set; }   // true/false. A fixed particle does not move.
        public Color  Color         { get; set; }   // the color of the particle.
        private bool DeleteMe       { get; set; }   // indicates whether or not the particle should be removed from the simulation.


        // for simplicity, I normalize the gravitational constant to 1.
        public const double G = 1;

        // when you create a new Particle, these are the default values of the member variables.
        public Particle()
        {
            this.PositionX = 0;
            this.PositionY = 0;
            this.VelocityX = 0;
            this.VelocityY = 0;
            this.ForceX = 0;
            this.ForceY = 0;
            this.Mass = 1;
            this.Radius = 100;
            this.Fixed = false;
            this.Color = Color.Lime;
            this.DeleteMe = false;
        }


        //======================================================================
        // calculates and updates the velocity and position of each of the
        // [Particles] over the specified [Time]. The larger the value of [Time]
        // you give, the greater the simulation "error" will be. Perhaps in the
        // future I'll implement/find  a runge kutta extrapolation function to
        // reduce the error. For, linear is fine.
        //======================================================================
        public static void Update(List<Particle> Particles, double Time)
        {
            //------------------------------------------------------------------
            // calculate the net forces acting on all particles
            //------------------------------------------------------------------
            // return all particles' forces to zero before recalculating
            foreach (Particle p in Particles)
            {
                p.ForceX = 0;
                p.ForceY = 0;
            }
            // calculate net forces acting on all particles
            int c = Particles.Count();
            for (int p1 = 0; p1 < c-1; p1++)
            {
                for(int p2 = p1+1; p2 < c; p2++)
                {
                    CalculateForcesBetweenTwoParticlesAndSum(Particles[p1], Particles[p2]);
                }
            }

            //------------------------------------------------------------------
            // calculate the new velocity of each particle
            //------------------------------------------------------------------
            foreach (Particle p in Particles)
            {
                p.VelocityX += Time * p.ForceX / p.Mass;
                p.VelocityY += Time * p.ForceY / p.Mass;
            }

            //------------------------------------------------------------------
            // calculate the new positions of each particle
            //------------------------------------------------------------------
            foreach (Particle p in Particles)
            {
                if (!p.Fixed)
                {
                    p.PositionX += Time * p.VelocityX;
                    p.PositionY += Time * p.VelocityY;
                }
            }
        }



        //======================================================================
        // this is meant to update the position of a single particle (called the
        // Projectile). The projectile is acted upon by a list of particles
        // called Targets. This function spends much fewer resources on updating
        // the projectile that the Update() function does.
        //======================================================================
        public static void UpdateSingle(Particle Projectile, List<Particle> Targets, double Time)
        {
            // initialize the Projectile's forces to 0.
            Projectile.ForceX = 0;
            Projectile.ForceY = 0;

            // calculate the net force acting on the Projectile
            foreach (Particle targ in Targets)
            {
                CalculateForcesBetweenTwoParticlesAndSum(Projectile, targ);
            }

            // calculate the new velocity of the Projectile
            Projectile.VelocityX += Time * Projectile.ForceX / Projectile.Mass;
            Projectile.VelocityY += Time * Projectile.ForceY / Projectile.Mass;
            
            // calculate the new positions of the Projectile
            Projectile.PositionX += Time * Projectile.VelocityX;
            Projectile.PositionY += Time * Projectile.VelocityY;
        }


        //======================================================================
        // this calculates the forces between two particles, and adds those
        // forces to the ForceX and ForceY components in both Particle objects.
        //======================================================================
        public static void CalculateForcesBetweenTwoParticlesAndSum(Particle p1, Particle p2)
        {
            // calculate the distance between the objects squared
            // Note: writing a manual square-ing function (instead of using Math.Pow() function) provides a ~17% increase in speed!
            double dist_squared =
                (p1.PositionX - p2.PositionX) * (p1.PositionX - p2.PositionX) +
                (p1.PositionY - p2.PositionY) * (p1.PositionY - p2.PositionY);

            double force;
            // calculate the magnitude of the force normally.
            force = Particle.G * p1.Mass * p2.Mass / dist_squared;

            // calculate the angle (relative to positive x direction, A.K.A. the i unit vector)
            double angle = Math.Atan2(
                p1.PositionY - p2.PositionY,
                p1.PositionX - p2.PositionX);

            // calculate the x- and y-components of the force.
            double FX = Math.Cos(angle) * force;
            double FY = Math.Sin(angle) * force;
            // apply forces to particle 2
            p2.ForceX += FX;
            p2.ForceY += FY;
            // forces are equal and opposite on particle 1
            p1.ForceX += -FX;
            p1.ForceY += -FY;
        }


        // this function will check all particles in the list.
        // it resolves all collisions.
        public static void CollisionCheckAndResolveAll(List<Particle> Particles)
        {
            bool FoundCollision;
            do
            {
                FoundCollision = false;
                // calculate net forces acting on all particles
                int c = Particles.Count();
                if(c > 1)
                {
                    for (int p1 = 0; ((p1 < c - 1) && (!FoundCollision)); p1++)
                    {
                        for (int p2 = p1 + 1; ((p2 < c) && (!FoundCollision)); p2++)
                        {
                            // if the particles touch, or are overlapping
                            if (Particle.CollisionCheck(Particles[p1], Particles[p2]))
                            {
                                Particle.Collision(Particles, Particles[p1], Particles[p2]);
                                FoundCollision = true;
                            }
                        }
                    }
                }
            }
            while (FoundCollision);
        }


        // this function will tell you if two particles are touching
        public static bool CollisionCheck(Particle p1, Particle p2)
        {
            // calculate the distance between the objects squared
            double dist_squared =
                Math.Pow(p1.PositionX - p2.PositionX, 2) +
                Math.Pow(p1.PositionY - p2.PositionY, 2);
            // if the distance squared between the two objects is less than or equal to the
            // sum of the squares of the two radii, then the two objects are touching.
            return (dist_squared <= Math.Pow(p1.Radius + p2.Radius, 2));
        }


        // call this function when two particles collide!
        private static void Collision(List<Particle> Particles, Particle p1, Particle p2)
        {
            // p1 becomes the combination of p1 and p2 in terms of mass, position, and momentum.
            // (p1 + p2) -> p1

            if(p1.Fixed)
            {
                p1.Mass += p2.Mass;
                Particles.Remove(p2);
            }
            else if(p2.Fixed)
            {
                p2.Mass += p1.Mass;
                Particles.Remove(p1);
            }
            // if neither particle is fixed,
            else
            {
                // calculate the net position of the combined particle
                double p1m = p1.Mass / (p1.Mass + p2.Mass);
                double p2m = p2.Mass / (p1.Mass + p2.Mass);
                p1.PositionX = p1m * p1.PositionX + p2m * p2.PositionX;
                p1.PositionY = p1m * p1.PositionY + p2m * p2.PositionY;

                // calculate net momentum from two particles
                double momentum_x = p1.VelocityX * p1.Mass + p2.VelocityX * p2.Mass;
                double momentum_y = p1.VelocityY * p1.Mass + p2.VelocityY * p2.Mass;

                // combine the masses of the two particles
                p1.Mass = p1.Mass + p2.Mass;

                // calculate new velocity of combined particle
                p1.VelocityX = momentum_x / p1.Mass;
                p1.VelocityY = momentum_y / p1.Mass;

                Particles.Remove(p2);
            }
        }



        // this draws all the particles in the list on the graphics object.
        public static void Draw(List<Particle> Particles, Graphics screen)
        {
            Pen myPen;
            float x, y, w, h;
            foreach (Particle p in Particles)
            {
                myPen = new Pen(p.Color);

                // draw a circle for the "body" of the particle
                x = (float)(p.PositionX - p.Radius);
                y = (float)(p.PositionY - p.Radius);
                w = (float)(2 * p.Radius);
                h = (float)(2 * p.Radius);
                screen.DrawEllipse(myPen, x, y, w, h);

                // draw a line showing the direction of the force extered on the particle
                double force_mag = Math.Sqrt(p.ForceX * p.ForceX + p.ForceY * p.ForceY);
                double force_x_norm = p.ForceX / force_mag;
                double force_y_norm = p.ForceY / force_mag;
                screen.DrawLine(myPen, (float)p.PositionX, (float)p.PositionY, (float)(p.PositionX + 2 * p.Radius * force_x_norm), (float)(p.PositionY + 2 * p.Radius * force_y_norm));

                // draw a line showing the direction the particle is moving
                double velocity_mag = Math.Sqrt(p.VelocityX * p.VelocityX + p.VelocityY * p.VelocityY);
                double velocity_x_norm = p.VelocityX / velocity_mag;
                double velocity_y_norm = p.VelocityY / velocity_mag;
                screen.DrawLine(myPen, (float)p.PositionX, (float)p.PositionY, (float)(p.PositionX + p.Radius * velocity_x_norm), (float)(p.PositionY + p.Radius * velocity_y_norm));


                myPen.Dispose();
            }
        }

    }
}