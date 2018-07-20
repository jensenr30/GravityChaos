using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace GravityChaos
{
    //======================================================================
    // contains all properties and methods necessary to render GravityChaos images.
    // see https://github.com/jensenr30/GravityChaos#how-it-works for an
    // explanation of how GravityChaos images are rendered.
    //======================================================================
    class GravityChaosRenderer
    {
        // this is the particle that is placed at different locations and is
        // allowed to move around until it hits a target
        public Particle Projectile { get; set; }
        // these are the target particles that the projectile can hit during
        // its travels. All targets are stationary for a given render.
        public List<Particle> Targets { get; set; }
        
        // this indicates the maximum number of iterations (simulation steps)
        // that can be performed while rendering any single pixel.
        // e.g. if (for a single pixel at <x,y>) this many steps have been
        // calculated and the projectile has still not hit any of the targets,
        // the simulation will be terminated and the pixel at <x,y> will be
        // colored the default color.
        public double IterationsMax { get; set; }
        // this is the default color. During the rendering process, this color
        // is used when a particle does not hit any target.
        public Color ColorDefault { get; set; }
        // this indicates how big of a step (in time) will be taken for each
        // iteration of the simulation. The size of a step in space is proportional
        // to this value. When this is set high, you will get "aliasing". An
        // example of aliasing can be see in this image:
        // https://github.com/jensenr30/GravityChaos/raw/master/pics/snow%20drift%20high%20res.png?raw=true
        // the aliasing can easily be identified by the tell-tale rings around
        // each of the five targets.
        public double IterationTimeStep { get; set; }
        
        // this defines the area (in the space of the projectile and targets)
        // that will be rendered.
        // NOTE: the space you choose to render will be
        // stretched into the Image's Height x Width.
        // i.e., the space defined by SpaceToRender will be fit into whatever
        // pixel-space defined by ImageHeight and ImageWidth - by design choice,
        // the aspect ratio may be altered in this transformation. 
        public ParticleSpace SpaceToRender { get; set; }
        // this is how wide the rendered image will be [pixels]
        public int ImageWidth
        {
            get
            {
                return ImageWidth;
            }
            set
            {
                if (!IsCurrentlyRendering)
                    ImageWidth = value;
                else
                    throw new Exception("Cannot change ImageWidth! GravityChaosRenderer is currently rendering the image!");
            }
        }
        // this is how tall the rendered image will be [pixels]
        public int ImageHeight
        {
            get
            {
                return ImageHeight;
            }
            set
            {
                if (!IsCurrentlyRendering)
                    ImageHeight = value;
                else
                    throw new Exception("Cannot change ImageHeight! GravityChaosRenderer is currently rendering the image!");
            }
        }
        // this is the bitmap upon which the image will be rendered.
        public Bitmap Image { get; private set; }
        // This is the current pixel being rendered <x,y>
        private int ImagePixelX { get; set; }
        private int ImagePixelY { get; set; }
        
        // this records when the rendering started
        private DateTime TimeRenderingStarted { get; set; }
        // this records when the rendering stopped
        private DateTime TimeRenderingStopped { get; set; }
        // this indicates that the rendering process is currently running
        public bool IsCurrentlyRendering { get; private set; }
        
        // this tells you what percentage of the image has been rendered so far.
        // yields a number between 0.0 and 100.0.
        public double PercentComplete
        {
            get
            {
                return ((ImagePixelY / (double)ImageHeight) + (ImagePixelX / (double)(ImageHeight * ImageWidth))) * 100.0;
            }
        }
        
        // this returns the total time (milliseconds) that has been used to render the image
        public int TotalRenderTime
        {
            get
            {
                if (IsCurrentlyRendering)
                {
                    return (int)(DateTime.Now - TimeRenderingStarted).TotalMilliseconds;
                }
                else
                {
                    return (int)(TimeRenderingStopped - TimeRenderingStarted).TotalMilliseconds;
                }
            }
        }


        //======================================================================
        // convert an X coordinate in image-space to particle-space
        //======================================================================
        public double ImageSpaceToParticleSpaceX(int x)
        {
            return SpaceToRender.X + x * SpaceToRender.Width/(double)ImageWidth;
        }


        //======================================================================
        // convert a  Y coordinate in image-space to particle-space
        //======================================================================
        public double ImageSpaceToParticleSpaceY(int y)
        {
            return SpaceToRender.Y + y * SpaceToRender.Height / (double)ImageHeight;
        }


        //======================================================================
        // default values are assigned in constructor
        //======================================================================
        public GravityChaosRenderer()
        {
            Projectile.Mass = 1;
            Projectile.Radius = 0;

            IterationsMax = 4;
            ColorDefault = Color.Black;
            IterationTimeStep = 1;

            SpaceToRender = new ParticleSpace(-50, 50, 50, 50);
            ImageWidth = 480;
            ImageHeight = 270;

            TimeRenderingStarted = DateTime.MinValue;
            TimeRenderingStopped = DateTime.MinValue;
            IsCurrentlyRendering = false;
        }

        
        //======================================================================
        // this function renders the image using the existing configuration of
        // the properties of this class.
        //======================================================================
        public Bitmap Render()
        {

            IsCurrentlyRendering = true;
            Image.Dispose();
            Image = new Bitmap(ImageWidth, ImageHeight);

            TimeRenderingStarted = DateTime.Now;
            bool collision;
            int iterations;
            for (ImagePixelX = 0; ImagePixelX < ImageWidth; ImagePixelX++)
            {
                for (ImagePixelY = 0; ImagePixelY < ImageHeight; ImagePixelY++)
                {
                    collision = false;
                    iterations = 0;
                    while((!collision) && (iterations < IterationsMax))
                    {
                        // TODO: try for instead of foreach to try to get a reduction in render time.
                        // check to see if the moving particle has hit any of the targets
                        foreach (Particle p in Targets)
                        {
                            collision = Particle.CollisionCheck(Projectile, p);
                            // if the particle collided, set the pixel to the appropriate color.
                            if (collision)
                            {
                                Image.SetPixel(ImagePixelX, ImagePixelY, p.Color);
                                break;
                            }
                        }
                        // TODO: try putting an if statement around this, checking if the collision happened. if ther was a collision, we don't have to update the map! we can just quit.
                        // run the simulation one iteration
                        Particle.UpdateSingle(Projectile, Targets, IterationTimeStep);
                        iterations++;
                    }
                    // if the Projectile didn't hit any Targets, use default color
                    // TODO: this can be replaced by initializing the Image to be 100% this default color from the start. it would clean up this loop.
                    if (!collision)
                    {
                        Image.SetPixel(ImagePixelX, ImagePixelY, ColorDefault);
                    }
                }
            }


            IsCurrentlyRendering = false;
            TimeRenderingStopped = DateTime.Now;
            return Image;
        }
        
        
        //======================================================================
        // export the image as a PNG into the render/ subfolder of the program's
        // working directory.
        //======================================================================
        public void ExportImage()
        {
            ExportImage("render/" + (DateTime.Now).ToString("yyyy-MM-ddTHH.mm.ss") + ".png");
        }


        //======================================================================
        // export the bitmap stored in the Image property of this class to the
        // specified file location.
        //======================================================================
        public void ExportImage(string fileName)
        {
            ExportImage(fileName, System.Drawing.Imaging.ImageFormat.Png);
        }


        //======================================================================
        // export the bitmap stored in the Image property of this class to the
        // specified file location, with the specified image format.
        //======================================================================
        public void ExportImage(string fileName, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            Image.Save(fileName, imageFormat);
        }
    }




    //======================================================================
    // defines a rectangular area in particle space
    //======================================================================
    class ParticleSpace
    {
        public double X { get; set; }   // x position
        public double Y { get; set; }   // y position
        public double Width { get; set; }   // width of the space
        public double Height { get; set; }   // height of the space

        public ParticleSpace()
        {

        }

        public ParticleSpace(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    
    
    //======================================================================
    // contains all fields to specify a particle
    // also contains all methods necessary to perform operations on particles
    //======================================================================
    class Particle
    {
        //======================================================================
        // define parameters of the particle
        //======================================================================
        public double PositionX     { get; set; }
        public double PositionY     { get; set; }
        public double VelocityX     { get; set; }
        public double VelocityY     { get; set; }
        public double ForceX        { get; set; }
        public double ForceY        { get; set; }
        public double Mass          { get; set; }
        public double Radius        { get; set; }
        public bool   Fixed         { get; set; }
        public Color  Color         { get; set; }


        //======================================================================
        // Particle constructor
        //======================================================================
        public Particle()
        {
            // default values of a particle
            PositionX = 0;
            PositionY = 0;
            VelocityX = 0;
            VelocityY = 0;
            ForceX = 0;
            ForceY = 0;
            Mass = 1;
            Radius = 100;
            Fixed = false;
            Color = Color.Lime;
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
            foreach (Particle Target in Targets)
            {
                CalculateForcesBetweenTwoParticlesAndSumSingle(Projectile, Target);
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
        // forces to the ForceX and ForceY components in ONLY one of the
        // particles (the first argument, projectile).
        //======================================================================
        public static void CalculateForcesBetweenTwoParticlesAndSumSingle(Particle projectile, Particle target)
        {
            // calculate the distance between the objects squared
            // Note: writing a manual square-ing function (instead of using Math.Pow() function) provides a ~17% increase in speed!
            double dist_squared =
                (projectile.PositionX - target.PositionX) * (projectile.PositionX - target.PositionX) +
                (projectile.PositionY - target.PositionY) * (projectile.PositionY - target.PositionY);

            double force;
            // calculate the magnitude of the force normally.
            // the gravitational constant is left of of this formula (i.e. normalized to 1 for sake of simpler (perhaps slightly faster?) calculations)
            force = projectile.Mass * target.Mass / dist_squared;

            // calculate the angle (relative to positive x direction, A.K.A. the i unit vector)
            double angle = Math.Atan2(
                projectile.PositionY - target.PositionY,
                projectile.PositionX - target.PositionX);

            // calculate the x- and y-components of the force.
            double FX = Math.Cos(angle) * force;
            double FY = Math.Sin(angle) * force;

            // only add the forces of this interaction to projectile.
            projectile.ForceX -= FX;
            projectile.ForceY -= FY;
        }


        //======================================================================
        // this function will tell you if two particles are touching
        //======================================================================
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

        
        #region unused code

        ////======================================================================
        //// calculates and updates the velocity and position of each of the
        //// [Particles] over the specified [Time]. The larger the value of [Time]
        //// you give, the greater the simulation "error" will be. Perhaps in the
        //// future I'll implement/find  a runge kutta extrapolation function to
        //// reduce the error. For, linear is fine.
        ////======================================================================
        //public static void Update(List<Particle> Particles, double Time)
        //{
        //    //------------------------------------------------------------------
        //    // calculate the net forces acting on all particles
        //    //------------------------------------------------------------------
        //    // return all particles' forces to zero before recalculating
        //    foreach (Particle p in Particles)
        //    {
        //        p.ForceX = 0;
        //        p.ForceY = 0;
        //    }
        //    // calculate net forces acting on all particles
        //    int c = Particles.Count();
        //    for (int p1 = 0; p1 < c - 1; p1++)
        //    {
        //        for (int p2 = p1 + 1; p2 < c; p2++)
        //        {
        //            CalculateForcesBetweenTwoParticlesAndSum(Particles[p1], Particles[p2]);
        //        }
        //    }

        //    //------------------------------------------------------------------
        //    // calculate the new velocity of each particle
        //    //------------------------------------------------------------------
        //    foreach (Particle p in Particles)
        //    {
        //        p.VelocityX += Time * p.ForceX / p.Mass;
        //        p.VelocityY += Time * p.ForceY / p.Mass;
        //    }

        //    //------------------------------------------------------------------
        //    // calculate the new positions of each particle
        //    //------------------------------------------------------------------
        //    foreach (Particle p in Particles)
        //    {
        //        if (!p.Fixed)
        //        {
        //            p.PositionX += Time * p.VelocityX;
        //            p.PositionY += Time * p.VelocityY;
        //        }
        //    }
        //}

        
        ////======================================================================
        //// this calculates the forces between two particles, and adds those
        //// forces to the ForceX and ForceY components in both Particle objects.
        ////======================================================================
        //public static void CalculateForcesBetweenTwoParticlesAndSum(Particle p1, Particle p2)
        //{
        //    // calculate the distance between the objects squared
        //    // Note: writing a manual square-ing function (instead of using Math.Pow() function) provides a ~17% increase in speed!
        //    double dist_squared =
        //        (p1.PositionX - p2.PositionX) * (p1.PositionX - p2.PositionX) +
        //        (p1.PositionY - p2.PositionY) * (p1.PositionY - p2.PositionY);

        //    double force;
        //    // calculate the magnitude of the force normally.
        //    // the gravitational constant is left of of this formula (i.e. normalized to 1 for sake of simpler (perhaps slightly faster?) calculations)
        //    force = p1.Mass * p2.Mass / dist_squared;

        //    // calculate the angle (relative to positive x direction, A.K.A. the i unit vector)
        //    double angle = Math.Atan2(
        //        p1.PositionY - p2.PositionY,
        //        p1.PositionX - p2.PositionX);

        //    // calculate the x- and y-components of the force.
        //    double FX = Math.Cos(angle) * force;
        //    double FY = Math.Sin(angle) * force;
        //    // apply forces to particle 2
        //    p2.ForceX += FX;
        //    p2.ForceY += FY;
        //    // forces are equal and opposite on particle 1
        //    p1.ForceX += -FX;
        //    p1.ForceY += -FY;
        //}


        //// this function will check all particles in the list.
        //// it resolves all collisions.
        //public static void CollisionCheckAndResolveAll(List<Particle> Particles)
        //{
        //    bool FoundCollision;
        //    do
        //    {
        //        FoundCollision = false;
        //        // calculate net forces acting on all particles
        //        int c = Particles.Count();
        //        if (c > 1)
        //        {
        //            for (int p1 = 0; ((p1 < c - 1) && (!FoundCollision)); p1++)
        //            {
        //                for (int p2 = p1 + 1; ((p2 < c) && (!FoundCollision)); p2++)
        //                {
        //                    // if the particles touch, or are overlapping
        //                    if (Particle.CollisionCheck(Particles[p1], Particles[p2]))
        //                    {
        //                        Particle.Collision(Particles, Particles[p1], Particles[p2]);
        //                        FoundCollision = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    while (FoundCollision);
        //}


        //// call this function when two particles collide!
        //private static void Collision(List<Particle> Particles, Particle p1, Particle p2)
        //{
        //    // p1 becomes the combination of p1 and p2 in terms of mass, position, and momentum.
        //    // (p1 + p2) -> p1

        //    if (p1.Fixed)
        //    {
        //        p1.Mass += p2.Mass;
        //        Particles.Remove(p2);
        //    }
        //    else if (p2.Fixed)
        //    {
        //        p2.Mass += p1.Mass;
        //        Particles.Remove(p1);
        //    }
        //    // if neither particle is fixed,
        //    else
        //    {
        //        // calculate the net position of the combined particle
        //        double p1m = p1.Mass / (p1.Mass + p2.Mass);
        //        double p2m = p2.Mass / (p1.Mass + p2.Mass);
        //        p1.PositionX = p1m * p1.PositionX + p2m * p2.PositionX;
        //        p1.PositionY = p1m * p1.PositionY + p2m * p2.PositionY;

        //        // calculate net momentum from two particles
        //        double momentum_x = p1.VelocityX * p1.Mass + p2.VelocityX * p2.Mass;
        //        double momentum_y = p1.VelocityY * p1.Mass + p2.VelocityY * p2.Mass;

        //        // combine the masses of the two particles
        //        p1.Mass = p1.Mass + p2.Mass;

        //        // calculate new velocity of combined particle
        //        p1.VelocityX = momentum_x / p1.Mass;
        //        p1.VelocityY = momentum_y / p1.Mass;

        //        Particles.Remove(p2);
        //    }
        //}



        //// this draws all the particles in the list on the graphics object.
        //public static void Draw(List<Particle> Particles, Graphics screen)
        //{
        //    Pen myPen;
        //    float x, y, w, h;
        //    foreach (Particle p in Particles)
        //    {
        //        myPen = new Pen(p.Color);

        //        // draw a circle for the "body" of the particle
        //        x = (float)(p.PositionX - p.Radius);
        //        y = (float)(p.PositionY - p.Radius);
        //        w = (float)(2 * p.Radius);
        //        h = (float)(2 * p.Radius);
        //        screen.DrawEllipse(myPen, x, y, w, h);

        //        // draw a line showing the direction of the force extered on the particle
        //        double force_mag = Math.Sqrt(p.ForceX * p.ForceX + p.ForceY * p.ForceY);
        //        double force_x_norm = p.ForceX / force_mag;
        //        double force_y_norm = p.ForceY / force_mag;
        //        screen.DrawLine(myPen, (float)p.PositionX, (float)p.PositionY, (float)(p.PositionX + 2 * p.Radius * force_x_norm), (float)(p.PositionY + 2 * p.Radius * force_y_norm));

        //        // draw a line showing the direction the particle is moving
        //        double velocity_mag = Math.Sqrt(p.VelocityX * p.VelocityX + p.VelocityY * p.VelocityY);
        //        double velocity_x_norm = p.VelocityX / velocity_mag;
        //        double velocity_y_norm = p.VelocityY / velocity_mag;
        //        screen.DrawLine(myPen, (float)p.PositionX, (float)p.PositionY, (float)(p.PositionX + p.Radius * velocity_x_norm), (float)(p.PositionY + p.Radius * velocity_y_norm));


        //        myPen.Dispose();
        //    }
        //}

        #endregion
    }
}