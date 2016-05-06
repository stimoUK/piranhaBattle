using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.
using System.Diagnostics;
using System.Threading;

/* LERNING PILL: XNAMachinationisRatio Engine
 * XNAMachinationisRatio is an engine that allows implementing
 * simulations and games based on XNA, simplifying the use of XNA
 * and adding features not directly available in XNA.
 * XNAMachinationisRatio is a work in progress.
 * The engine works "under the hood", taking care of many features
 * of an interactive simulation automatically, thus minimizing
 * the amount of code that developers have to write.
 * 
 * In order to use the engine, the application main class (Kernel, in the
 * case of FishO'Rama) creates, initializes and stores
 * an instance of class Engine in one of its data members.
 * 
 * The classes comprised in the  XNA Machinationis Ratio engine and the
 * related functionalities can be accessed from any of your XNA project
 * source code files by adding appropriate 'using' statements at the beginning of
 * the file. 
 * 
 */

namespace FishORama
{
    /* LEARNING PILL: Token behaviors in the XNA Machinationis Ratio engine
     * Some simulation tokens may need to enact specific behaviors in order to
     * participate in the simulation. The XNA Machinationis Ratio engine
     * allows a token to enact a behavior by associating an artificial intelligence
     * mind to it. Mind objects are created from subclasses of the class AIPlayer
     * included in the engine. In order to associate a mind to a token, a new
     * mind object must be created, passing to the constructor of the mind a reference
     * of the object that must be associated with the mind. This must be done in
     * the DefaultProperties method of the token.
     * 
     * Hence, every time a new tipe of AI mind is required, a new class derived from
     * AIPlayer must be created, and an instance of it must be associated to the
     * token classes that need it.
     * 
     * Mind objects enact behaviors through the method Update (see below for further details). 
     */
    class PiranhaMind : AIPlayer
    {

        #region Data Members

        // This mind needs to interact with the token which it possesses, 
        // since it needs to know where are the aquarium's boundaries.
        // Hence, the mind needs a "link" to the aquarium, which is why it stores in
        // an instance variable a reference to its aquarium.
        private AquariumToken mAquarium;        // Reference to the aquarium in which the creature lives.
        private int fishNum;
        private int teamNum;

        private float mFacingDirectionX = -1;         // Direction the fish is facing (1: right; -1: left).

        private float mSpeed = 5;
        public static bool win = false;
        private Vector3 chickLegVector;
        private Vector3 startPosition;
        private bool RemoveLegX = false;
        private bool RemoveLegY = false;
        private float rad = 15; // radius the piranha swim around.
        private float angle;
        private bool spawn = true;
        private Random rNum = new Random();
        private static int numInPlay;
        private static bool active = false;
        private static int team1Score;
        private static int team2Score;


        #endregion

        #region Properties

        /// <summary>
        /// Set Aquarium in which the mind's behavior should be enacted.
        /// </summary>
        public AquariumToken Aquarium
        {
            set { mAquarium = value; }
        }

        public int FishNum
        {
            set { fishNum = value; }
        }

        public int TeamNum
        {
            set { teamNum = value; }
        }

        /*public Vector3 StartPosition
        {
            set { startPosition = value; }
        }*/


        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public PiranhaMind(X2DToken pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            //mFacingDirectionX = 1;
            this.Possess(pToken);
        }

        #endregion

        #region Methods

        /* LEARNING PILL: The AI update method.
         * Mind objects enact behaviors through the method Update. This method is
         * automatically invoked by the engine, periodically, 'under the hood'. This can be
         * be better understood that the engine asks to all the available AI-based tokens:
         * "Would you like to do anything at all?" And this 'asking' is done through invoking
         * the Update method of each mind available in the system. The response is the execution
         * of the Update method of each mind , and all the methods possibly triggered by Update.
         * 
         * Although the Update method could invoke other methods if needed, EVERY
         * BEHAVIOR STARTS from Update. If a behavior is not directly coded in Updated, or in
         * a method invoked by Update, then it is IGNORED.
         * 
         */

        #region  RandomPiranha

        public void RandomPiranha()
        {
            // will only do when fish is not active.
            if (!active)
            {
                numInPlay = rNum.Next(1, 4);
                //Console.WriteLine(numInPlay);
            }
        }

        #endregion

        #region CircleSwimBehaviour

        public void CircleSwimBehaviour()
        {
            angle += 0.05f;
            Vector3 tokenPosition = this.PossessedToken.Position;
            //position.X,Y = cos/sin(angle) * radius + origin.
            tokenPosition.Y = (float)Math.Cos(angle) * rad + startPosition.Y;//works with tokenPosition.
            tokenPosition.X = (float)Math.Sin(angle) * rad + startPosition.X;
            this.PossessedToken.Position = tokenPosition;
            this.PossessedToken.Orientation = new Vector3(mFacingDirectionX,
                                                        this.PossessedToken.Orientation.Y,
                                                        this.PossessedToken.Orientation.Z);
       
            //http://gamedev.stackexchange.com/questions/37462/circular-movement-eliminating-speed-ups-near-y-0
        }

        #endregion

        #region FeedingBehaviour

        public void FeedingBehaviour()
        {
            Vector3 tokenPosition = this.PossessedToken.Position;
            //This is giving the ChickenLeg and the tokenPosition a vector3 name of chickLegVector
            chickLegVector = mAquarium.ChickenLeg.Position - tokenPosition;
            /*Normalize is changing the vector from a length to a direction which then 
                the piranha as a point to go to. 
                researced at http://xnafan.net/2012/12/pointing-and-moving-towards-a-target-in-xna-2d/ */
            chickLegVector.Normalize();
            tokenPosition += (chickLegVector * mSpeed);

            //Deleting the chickenleg           
            //if X coor and chickenleg are within 5px of each other then the X coor is removed.
            if (tokenPosition.X - mAquarium.ChickenLeg.Position.X <= 5 && tokenPosition.X - mAquarium.ChickenLeg.Position.X >= -5)
            {
                RemoveLegX = true;
            }

            if (tokenPosition.Y - mAquarium.ChickenLeg.Position.Y <= 5 && tokenPosition.Y - mAquarium.ChickenLeg.Position.Y >= -5)
            {
                RemoveLegY = true;
            }

            /*If both the X and Y coordinates are removed then the chicken leg is removed
                and the remove legs are reset and the feed = true*/
            if (RemoveLegX && RemoveLegY == true)
            {
                mAquarium.RemoveChickenLeg();
                RemoveLegX = false;
                RemoveLegY = false;
                active = false;
                RandomPiranha();

                if (teamNum == 1)
                {
                    if (tokenPosition.X - chickLegVector.X <= 5 && tokenPosition.X - chickLegVector.X >= -5)
                    {
                        team1Score += 1;
                    }
                }
                else if (teamNum == 2)
                {
                    if (tokenPosition.X - chickLegVector.X <= 5 && tokenPosition.X - chickLegVector.X >= -5)
                    {
                        team2Score += 1;
                    }
                }
                Console.WriteLine("team 1 = {0}", team1Score);
                Console.WriteLine("team 2 = {0}", team2Score);
            }

            this.PossessedToken.Position = tokenPosition;
            this.PossessedToken.Orientation = new Vector3(mFacingDirectionX,
                                            this.PossessedToken.Orientation.Y,
                                            this.PossessedToken.Orientation.Z);

        }

        #endregion

        #region HorizontalSwimBehaviour

        public void HorizontalSwimBehaviour()
        {
            Vector3 tokenPosition = this.PossessedToken.Position;
            tokenPosition.X = tokenPosition.X + (mSpeed * mFacingDirectionX);
            this.PossessedToken.Position = tokenPosition;
            this.PossessedToken.Orientation = new Vector3(mFacingDirectionX,
                                                        this.PossessedToken.Orientation.Y,
                                                        this.PossessedToken.Orientation.Z);

            // delete this to make them swim off the screen, prefer them swimming around.
            // Reached Horizontal Boundary is defined in the Aquarium Token.
            if (this.mAquarium.ReachedHorizontalBoundary(this.PossessedToken))
            {
                // an alternative way of writing mFacingDirection * -1;
                mFacingDirectionX = -mFacingDirectionX;
            }

        }
        #endregion

        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            Vector3 tokenPosition = this.PossessedToken.Position;

            //when spawned startposition is remembered and their direction is set.
            if (spawn)
            {
                startPosition = tokenPosition;
                if (teamNum == 1)
                {
                    mFacingDirectionX = 1;
                }
                else if (teamNum == 2)
                {
                    mFacingDirectionX = -1;
                }
                spawn = false; 
            }
            
            // if win = false then do
            if (!win)
            {
                if (numInPlay == 0)
                {
                    RandomPiranha();
                }

                if (mAquarium.ChickenLeg != null && fishNum == numInPlay)
                {
                    active = true;
                    FeedingBehaviour();
                }
                else
                {
                    CircleSwimBehaviour();
                }
            }

            if (team1Score == 5)
            {
                win = true;
                if (teamNum == 1)
                {
                    HorizontalSwimBehaviour();
                }
                else if (teamNum == 2)
                {
                    CircleSwimBehaviour();
                }
            }
 
            if (team2Score == 5)
            {
                win = true;
                if (teamNum == 2)
                {
                    HorizontalSwimBehaviour();
                }
                else if (teamNum == 1)
                {
                    CircleSwimBehaviour();
                }
            }
        }


        #endregion
    }
}