using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;              // Required to use XNA features.
using XNAMachinationisRatio;                // Required to use the XNA Machinationis Ratio Engine general features.
using XNAMachinationisRatio.AI;             // Required to use the XNA Machinationis Ratio general AI features.

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
    class AquariumMind : AIPlayer
    {
        #region Data Members

        // This mind needs to interact with the token which it possesses, 
        // since it needs to know where are the aquarium's boundaries.
        // Hence, the mind needs a "link" to the aquarium, which is why it stores in
        // an instance variable a reference to its aquarium.
        private AquariumToken mAquarium = null;         // Reference to the aquarium in which the creature lives.

        //private float mFacingDirection;     // Direction the Aquarium is facing (1: right; -1: left).

        private Random rNum = new Random();
        private int rLeg;

        #endregion

        #region Properties

        /// <summary>
        /// Set Aquarium in which the mind's behavior should be enacted.
        /// </summary>
        public AquariumToken Aquarium
        {
            set { mAquarium = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pToken">Token to be associated with the mind.</param>
        public AquariumMind(X2DToken pToken)
        {
            /* LEARNING PILL: associating a mind with a token
             * In order for a mind to control a token, it must be associated with the token.
             * This is done when the mind is constructed, using the method Possess inherited
             * from class AIPlayer.
             */
            this.Possess(pToken);       // Possess token.

            //mFacingDirection = 1;       // Current direction the fish is facing.
        }

        #endregion

        #region Methods

        /// <summary>
        /// AI Update method.
        /// </summary>
        /// <param name="pGameTime">Game time</param>
        public override void Update(ref GameTime pGameTime)
        {
            PlaceLeg(); 
            //CheckLeftClickBehavior();
        }

        /// <summary>
        /// Check if the user clicks left mouse button. If the user clicked in the Aquarium,
        /// and if the leg is not already in the aquarium, place a new leg at click
        /// position.
        /// </summary>
        public void PlaceLeg()
        {
            if (!PiranhaMind.win)
            {
                rLeg = rNum.Next(0, 50);
                //Console.WriteLine(rLeg);

                if (rLeg == 1 && mAquarium.ChickenLeg == null)
                {

                    Vector2 chickLegPos = new Vector2(400, 300);
                    Vector3 legPos = this.mAquarium.Kernel.Camera.CameraToWorld(chickLegPos);

                    if ((Math.Abs(legPos.X) < mAquarium.Width / 2) && (Math.Abs(legPos.Y) < mAquarium.Height / 2))
                    {
                        mAquarium.ChickenLeg = new ChickenLegToken("ChickenLeg");
                        legPos.Z = 3;

                        this.mAquarium.Kernel.Scene.Place(mAquarium.ChickenLeg, legPos);
                    }
                }
            }
        }

        public void CheckLeftClickBehavior()
        {

            if ((Mouse.GetState().LeftButton == ButtonState.Pressed) && (mAquarium.ChickenLeg == null))
            {

                Vector2 mouseClickPos = new Vector2(400, 300);
                Vector3 legPos = this.mAquarium.Kernel.Camera.CameraToWorld(mouseClickPos);

                if ((Math.Abs(legPos.X) < mAquarium.Width / 2) && (Math.Abs(legPos.Y) < mAquarium.Height / 2))
                {
                
                    mAquarium.ChickenLeg = new ChickenLegToken("ChickenLeg");

                    legPos.Z = 3;

                    this.mAquarium.Kernel.Scene.Place(mAquarium.ChickenLeg, legPos);
                }
            }
        }

        #endregion
    }
}