using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BartlomiejJagielloLab1ZadDom
{
    public partial class FormMain : Form
    {
        #region Resource values
        /// <summary>
        /// Default maximum value for most in-game resources
        /// </summary>
        private const int ResourceValueLimit = 100000;

        /// <summary>
        /// Initial value for food resource
        /// </summary>
        private const int FoodStartingAmount = 100;
        /// <summary>
        /// Available to spend food resource
        /// </summary>
        private int foodAmount = FoodStartingAmount;
        /// <summary>
        /// Amount by which food resource will be increased each tick
        /// </summary
        private int foodIncrease = 20;

        /// <summary>
        /// Initial value for water resource
        /// </summary>
        private const int WaterStartingAmount = 100;
        /// <summary>
        /// Available to spend water resource
        /// </summary>
        private int waterAmount = WaterStartingAmount;
        /// <summary>
        /// Maximum value for water resource
        /// </summary>
        private const int WaterValueLimit = 100;
        /// <summary>
        /// Amount by which water resource will be increased each tick
        /// </summary
        private int waterIncrease = 20;

        /// <summary>
        /// Initial value for wood resource
        /// </summary>
        private const int WoodStartingAmount = 50;
        /// <summary>
        /// Available to spend wood resource
        /// </summary>
        private int woodAmount = WoodStartingAmount;
        /// <summary>
        /// Amount by which wood resource will be increased each tick
        /// </summary>
        private int woodIncrease = 10;

        /// <summary>
        /// Initial value for stone resource
        /// </summary>
        private const int StoneStartingAmount = 0;
        /// <summary>
        /// Available to spend stone resource
        /// </summary>
        private int stoneAmount = StoneStartingAmount;
        /// <summary>
        /// Amount by which stone resource will be increased each tick
        /// </summary>
        private int stoneIncrease = 0;

        /// <summary>
        /// Initial value for gold resource
        /// </summary>
        private const int GoldStartingAmount = 20;
        /// <summary>
        /// Available to spend gold resource
        /// </summary>
        private int goldAmount = GoldStartingAmount;
        /// <summary>
        /// Amount by which gold resource will be increased each tick
        /// </summary>
        private int goldIncrease = 1;

        /// <summary>
        /// Initial value for population resource
        /// </summary>
        private const int PopulationStartingAmount = 10;
        /// <summary>
        /// Value needed to win the game
        /// </summary>
        private const int PopulationWinningAmount = 500;
        /// <summary>
        /// Currently used population by buildings
        /// </summary>
        private int populationUsedAmount = 0;
        /// <summary>
        /// Available to spend population resource 
        /// </summary>
        private int populationAmount = PopulationStartingAmount;
        /// <summary>
        /// Total population resource
        /// </summary>
        private int populationSum = 15;
        /// <summary>
        /// Amount by which population resource will be increased each tick
        /// </summary>
        private int populationIncrease = 1;

        /// <summary>
        /// Initial value for faith resource
        /// </summary>
        private const int FaithStartingAmount = 25;
        /// <summary>
        /// Current faith resource value
        /// </summary>
        private int faithAmount = FaithStartingAmount;

        #endregion

        #region Ingame events
        /// <summary>
        /// Random numbers generator used for generating events and generating timeBeforeNextEvent
        /// </summary>
        private readonly Random RandomGenerator = new Random();

        /// <summary>
        /// Time (in timer ticks) between events
        /// Real time between events is +-20% from TimeBetweenEvents value
        /// </summary>
        private const int TimeBetweenEvents = 10;

        /// <summary>
        /// If reaches 0 next event will be randomly generated and new timeBeforeNextEvent will be set
        /// First event always occurs after 20 ticks
        /// Next depend on TimeBetweenEvents variable
        /// </summary>
        private int timeBeforeNextEvent = 20;

        /// <summary>
        /// Probability of WolvesAttackEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message, consumes 50 food resource and 2 population
        /// </summary>
        private readonly int[] WolvesAttackEvent = {0, 15};

        /// <summary>
        /// Probability of ShipDrownedEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message, consumes 20% stone resource and 20 gold resource
        /// </summary>
        private readonly int[] ShipDrownedEvent = {15, 25};

        /// <summary>
        /// Probability of RainEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message and does nothing
        /// </summary>
        private readonly int[] RainEvent = {25, 40};

        /// <summary>
        /// Probability of DiseaseEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message and consumes 25% population sum resource
        /// </summary>
        private readonly int[] DiseaseEvent = {40, 45};

        /// <summary>
        /// Probability of BanditsAttackEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message, consumes 50% food resource and 5 population sum resource
        /// </summary>
        private readonly int[] BanditsAttackEvent = {45, 55};

        /// <summary>
        /// Probability of DragonAttackEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message. consumes 100% food resource, 100% water resource, 100% wood resource and 20 population sum resource
        /// </summary>
        private readonly int[] DragonAttackEvent = {55, 60};

        /// <summary>
        /// Probability of FireEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message. consumes 100% water resource, 50% wood resource and 5 gold resource
        /// </summary>
        private readonly int[] FireEvent = {60, 75};

        /// <summary>
        /// Probability of NothingHappenedEvent occurring, represented by 2 values
        /// First is a lower bound (inclusive) needed for event to occur, second is high bound (exclusive)
        /// This event pops message and does nothing
        /// </summary>
        private readonly int[] NothingHappenedEvent = {75, 100};

        /// <summary>
        /// Represents all possible in game events
        /// </summary>
        private enum EventType: int
        {
         WolvesAttackEventType = 1, 
         ShipDrownedEventType = 2,
         RainEventType = 3,
         DiseaseEventType = 4,
         BanditsAttackEventType = 5,
         DragonAttackEventType = 6,
         FireEventType = 7,
         NothingHappenedEventType = 8
        }

        #region Event upgrades
        /// <summary>
        /// Represents if hunting upgrade is bought
        /// Changes wolves attack event behavior
        /// If true player gets 50 food and different message
        /// </summary>
        private bool huntingUpgradeActive = false;

        /// <summary>
        /// Represents if irrigation upgrade is bought
        /// Changes rain event behavior
        /// If true player gets 500 food resource and different message
        /// </summary>
        private bool irrigationUpgradeActive = false;

        /// <summary>
        /// Represents if walls upgrade is bought
        /// Changes bandits attack event behavior
        /// If true player gets different message
        /// </summary>
        private bool wallsUpgradeActive = false;

        /// <summary>
        /// Represents if navigation upgrade is bought
        /// Changes ship drowned event behavior
        /// If true player different message
        /// </summary>
        private bool navigationUpgradeActive = false;

        /// <summary>
        /// Represents if wells upgrade is bought
        /// Changes fire event behavior
        /// If true player loses 10% wood resource and gets different message
        /// </summary>
        private bool wellsUpgradeActive = false;

        /// <summary>
        /// Represents if medicine upgrade is bought
        /// Changes disease event behavior
        /// If true player loses 5% population and gets different message
        /// </summary>
        private bool medicineUpgradeActive = false;
        #endregion

        #endregion

        #region Game speed
        /// <summary>
        /// Timer game speed value
        /// Used when game speed is set to slow
        /// </summary>
        private const int SlowGameSpeed = 6000;

        /// <summary>
        /// Timer game speed value
        /// Used when game speed is set to normal
        /// Beginning game speed
        /// </summary>
        private const int NormalGameSpeed = 3000;

        /// <summary>
        /// Timer game speed value
        /// Used when game speed is set to fast
        /// </summary>
        private const int FastGameSpeed = 1500;

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Timer responsible for managing the flow of the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGameSpeed_Tick(object sender, EventArgs e)
        {
            // Add resources every tick
            IncrementResources();
            // Update displayed resources
            UpdateResourceLabels();
            // Decrease time before next event
            timeBeforeNextEvent -= 1;
            // When time before next event reaches 0 generate event
            if (timeBeforeNextEvent <= 0)
            {
                GenerateEvent();
            }
        }

        /// <summary>
        /// Function that stops game and prints message
        /// </summary>
        /// <param name="win">true if game is won; otherwise, false</param>
        private void EndGame(bool win)
        {
            timerGameSpeed.Stop();
            if (win)
            {
                // Show player new population value
                UpdateResourceLabels();
                MessageBox.Show("CONGRATULATIONS! YOU'VE WON!");
            }
            else
            {
                // Required to show player that population went to 0
                populationSum = 0;
                UpdateResourceLabels();
                MessageBox.Show("GAME OVER\nYOU LOSE");
            }
                
        }

        /// <summary>
        /// Randomly generates and executes event
        /// For all possible events see EventType
        /// Generates timeBeforeNextEvent for next event
        /// </summary>
        private void GenerateEvent()
        {
            int eventOccurred = RandomGenerator.Next(100);
            EventType eventType = EventType.NothingHappenedEventType;

            // Checks if NothingHappenedEvent occurred
            if (eventOccurred >= NothingHappenedEvent[0] && eventOccurred < NothingHappenedEvent[1])
                eventType = EventType.NothingHappenedEventType;
            // Checks if WolvesAttackEvent occurred
            else if (eventOccurred >= WolvesAttackEvent[0] && eventOccurred < WolvesAttackEvent[1])
                eventType = EventType.WolvesAttackEventType;
            // Checks if FireEvent occurred
            else if (eventOccurred >= FireEvent[0] && eventOccurred < FireEvent[1])
                eventType = EventType.FireEventType;
            // Checks if RainEvent occurred
            else if (eventOccurred >= RainEvent[0] && eventOccurred < RainEvent[1])
                eventType = EventType.RainEventType;
            //  Checks if ShipDrownedEvent occurred
            else if (eventOccurred >= ShipDrownedEvent[0] && eventOccurred < ShipDrownedEvent[1])
                eventType = EventType.ShipDrownedEventType;
            // Checks if BanditsAttackEvent occurred
            else if (eventOccurred >= BanditsAttackEvent[0] && eventOccurred < BanditsAttackEvent[1])
                eventType = EventType.BanditsAttackEventType;
            // Checks if DragonAttackEvent occurred
            else if (eventOccurred >= DragonAttackEvent[0] && eventOccurred < DragonAttackEvent[1])
                eventType = EventType.DragonAttackEventType;
            // Checks if DiseaseEvent occurred
            else if (eventOccurred >= DiseaseEvent[0] && eventOccurred < DiseaseEvent[1])
                eventType = EventType.DiseaseEventType;

            //Depending on event type display message in richTextBoxEventMessages and apply different effects
            switch (eventType)
            {
                case EventType.WolvesAttackEventType:
                    // If player bought hunting upgrade
                    if (huntingUpgradeActive)
                    {
                        // Give player 50 food
                        SetFoodAmount(foodAmount + 50);
                        richTextBoxEventsMessages.Text =
                            String.Format("Wolves came close to your village! Your people hunted some of them! You gain 50 food!");
                    }
                    else
                    {
                        // Reduce foodAmount by 50 and populationSum by 2
                        SetFoodAmount(foodAmount - 50);
                        SetPopulationSum(populationSum - 2);
                        richTextBoxEventsMessages.Text =
                            String.Format("Wolves attacked your villagers! You lost 50 food and 2 people!");
                    }
                    break;
                case EventType.ShipDrownedEventType:
                    // If player bought navigation upgrade
                    if (navigationUpgradeActive)
                    {
                        richTextBoxEventsMessages.Text =
                            String.Format("There was a scary storm on the sea, but all your people managed to get to safety!");
                    }
                    else
                    {
                        // Reduce stoneAmount by 20% and goldAmount by 20
                        int stoneLost = (int)(0.2 * stoneAmount);
                        SetStoneAmount(stoneAmount - stoneLost);
                        SetGoldAmount(goldAmount - 20);
                        richTextBoxEventsMessages.Text =
                            String.Format("Ship transporting stone to your village sank! You lost {0} stone and 20 gold!", stoneLost);
                    }
                    break;
                case EventType.RainEventType:
                    // If player bought irrigation upgrade
                    if (irrigationUpgradeActive)
                    {
                        // Give player 500 food resource
                        SetFoodAmount(foodAmount + 500);
                        richTextBoxEventsMessages.Text =
                            String.Format("It's raining! Your crops will be plentiful!");
                    }
                    else
                    {
                        richTextBoxEventsMessages.Text =
                            String.Format("It's rainy weekend");
                    }
                    break;
                case EventType.DiseaseEventType:
                    // If player bought medicine upgrade
                    if (medicineUpgradeActive)
                    {
                        // Reduce populationSum by 5%
                        int popLost = (int)(0.05 * populationSum);
                        SetPopulationSum(popLost);
                        richTextBoxEventsMessages.Text =
                            String.Format("Oh no! Your people have been infected with deadly disease. " +
                                          "Doctors managed to stopped the disease, but {0} people have died!", popLost);
                    }
                    else
                    {
                        // Reduce populationSum by 25%
                        int popLost = (int)(0.25 * populationSum);
                        SetPopulationSum(popLost);
                        richTextBoxEventsMessages.Text =
                            String.Format("Oh no! Your people have been infected with deadly disease. You lost {0} people!", popLost);
                    }
                    break;
                case EventType.BanditsAttackEventType:
                    int foodLost;
                    // If player bought walls upgrade
                    if (wallsUpgradeActive)
                    {
                        richTextBoxEventsMessages.Text =
                        String.Format("Bandits tried to attack your village! Thank god you built walls and your people are safe!");
                    }
                    else
                    {
                        // Reduce foodAmount by 50% and populationSum by 5
                        foodLost = (int)(0.5 * foodAmount);
                        SetFoodAmount(foodAmount - foodLost);
                        SetPopulationSum(populationSum - 5);
                        richTextBoxEventsMessages.Text =
                            String.Format("Your people have been attacked by bandits! " +
                                          "They stole {0} food and 5 of your people have been killed!", foodLost);
                    }
                    break;
                case EventType.DragonAttackEventType:
                    // Reduce foodAmount, waterAmount and woodAmount by 100% and populationSum by 20
                    foodLost = foodAmount;
                    SetFoodAmount(foodAmount - foodLost);
                    int waterLost = waterAmount;
                    SetWaterAmount(waterAmount - waterLost);
                    int woodLost = woodAmount;
                    SetWoodAmount(woodAmount - woodLost);
                    SetPopulationSum(populationSum - 20);
                    richTextBoxEventsMessages.Text =
                        String.Format("Dragon invaded you village and caused chaos! " +
                                      "You lost {0} food, {1} water, {2} wood and 20 population!", foodLost, waterLost, woodLost);
                    break;
                case EventType.FireEventType:
                    // If player bought wells upgrade
                    if (wellsUpgradeActive)
                    {
                        // Reduce waterAmount by 100% and woodAmount by 10%
                        waterLost = waterAmount;
                        SetWaterAmount(waterAmount - waterLost);
                        woodLost = (int)(0.1 * woodAmount);
                        SetWoodAmount(woodAmount - woodLost);
                        richTextBoxEventsMessages.Text =
                            String.Format("Your building are on fire! Thanks to many wells in your village, " +
                                          "repairs will only cost {0} water and {1} wood!", waterLost, woodLost);
                    }
                    else
                    {
                        // Reduce waterAmount by 100%, woodAmount by 50% and goldAmount by 5
                        waterLost = waterAmount;
                        SetWaterAmount(waterAmount - waterLost);
                        woodLost = (int)(0.5 * woodAmount);
                        SetWoodAmount(woodAmount - woodLost);
                        SetGoldAmount(goldAmount - 5);
                        richTextBoxEventsMessages.Text =
                            String.Format("Your building are on fire! Repairs will cost {0} water, {1} wood and 5 gold!", waterLost, woodLost);
                    }
                    break;
                case EventType.NothingHappenedEventType:
                    richTextBoxEventsMessages.Text =
                        String.Format("Nothing happened");
                    break;
            }
            // Update labels so user can see what event did
            UpdateResourceLabels();

            // Calculate new timeBeforeNextEvent equal to +-20% of TimeBetweenEvents
            timeBeforeNextEvent = (int)(TimeBetweenEvents*(RandomGenerator.NextDouble()*(1.2-0.8)+0.8));
        }
        
        /// <summary>
        /// Updates displayed resource values
        /// </summary>
        private void UpdateResourceLabels()
        {
            // Set resource labels text field to current resources values 
            labelFoodAmount.Text = foodAmount.ToString();
            labelWaterAmount.Text = waterAmount.ToString();
            labelWoodAmount.Text = woodAmount.ToString();
            labelStoneAmount.Text = stoneAmount.ToString();
            labelGoldAmount.Text = goldAmount.ToString();
            // Set population label text field to text in format population amount / population sum
            labelPopulationAmount.Text = String.Format("{0}/{1}", populationAmount, populationSum);
            // Update gold increase
            labelGoldIncrease.Text = String.Format("+{0}", goldIncrease.ToString());
        }
        
        /// <summary>
        /// Increases resource values every tick
        /// It should only be called by main game time (timerGameSpeed)
        /// </summary>
        private void IncrementResources()
        {
            // Increase every resource value by it's increase value
            SetFoodAmount(foodAmount + foodIncrease);
            SetWaterAmount(waterAmount + waterIncrease);
            SetWoodAmount(woodAmount + woodIncrease);
            SetStoneAmount(stoneAmount + stoneIncrease);
            // Earned gold depends on faith
            SetGoldAmount(goldAmount + (faithAmount / 25) * goldIncrease);
            // Increase population only if food and water resources increase are positive
            if (foodIncrease > 0 && waterIncrease > 0)
            {
                populationIncrease = 1;
                SetPopulationAmount(populationAmount + populationIncrease);
            }
            else
                populationIncrease = -1;
            UpdateLabelPopulationIncrease();
            }

        /// <summary>
        /// Changes Food resource value to given value only if new value is not negative
        /// Checks if food resource value is within maximum allowed value
        /// Returns false if newValue is negative, else returns true
        /// If newValue is negative call
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetFoodAmount(int newValue)
        {
            if (newValue < 0)
                return false;
            foodAmount = newValue;
            if (foodAmount > ResourceValueLimit)
                foodAmount = ResourceValueLimit;
            return true;
        }

        /// <summary>
        /// Changes water resource value to given value only if new value is not negative
        /// Checks if water resource value is within maximum allowed value
        /// Returns false if newValue is negative, else returns true
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetWaterAmount(int newValue)
        {
            if (newValue < 0)
                return false;
            waterAmount = newValue;
            if (waterAmount > WaterValueLimit)
                waterAmount = WaterValueLimit;
            return true;
        }

        /// <summary>
        /// Changes wood resource value to given value only if new value is not negative
        /// Checks if wood resource value is within maximum allowed value
        /// Returns false if newValue is negative, else returns true
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetWoodAmount(int newValue)
        {
            if (newValue < 0)
                return false;
            woodAmount = newValue;
            if (woodAmount > ResourceValueLimit)
                woodAmount = ResourceValueLimit;
            return true;
        }

        /// <summary>
        /// Changes stone resource value to given value only if new value is not negative
        /// Checks if stone resource value is within maximum allowed value
        /// Returns false if newValue is negative, else returns true
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetStoneAmount(int newValue)
        {
            if (newValue < 0)
                return false;
            stoneAmount = newValue;
            if (stoneAmount > ResourceValueLimit)
                stoneAmount = ResourceValueLimit;
            return true;
        }

        /// <summary>
        /// Changes gold resource value to given value only if new value is not negative
        /// Checks if gold resource value is within maximum allowed value
        /// Returns false if newValue is negative, else returns true
        /// Depends on faith amounth
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetGoldAmount(int newValue)
        {
            if (newValue < 0)
                return false;
            goldAmount = newValue;
            if (goldAmount > ResourceValueLimit)
                goldAmount = ResourceValueLimit;
            return true;
        }

        /// <summary>
        /// Changes population resource value to given value only if new value is not negative
        /// Checks if population resource value is less or equal population sum value minus population used amount 
        /// Returns false if newValue is negative, else returns true
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetPopulationAmount(int newValue)
        {
            if (newValue < 0)
                return false;
            // Temporary variable to store how many population was added (if any)
            int populationAdded = 0;
            // populationAmount is capped at populationSum-populationUsedAmount
            if (newValue > populationSum - populationUsedAmount)
            {
                // Used to calculate difference in population, subtract old value
                populationAdded -= populationAmount;
                populationAmount = populationSum - populationUsedAmount;
                // Used to calculate difference in population, add new value
                populationAdded += populationAmount;
            }
            else
            {
                // Used to calculate difference in population, subtract old value
                populationAdded -= populationAmount;
                populationAmount = newValue;
                // Used to calculate difference in population, add new value
                populationAdded += populationAmount;
            }
            // If population was added decrease gained food and water resources per tick and update displayed values
            if (populationAdded > 0)
            {
                foodIncrease -= populationAdded;
                UpdateLabelFoodIncrease();
                waterIncrease -= populationAdded;
                UpdateLabelWaterIncrease();
            }
            return true;
        }

        /// <summary>
        /// Changes population sum to given value
        /// Checks if population sum is within maximum allowed value
        /// Calls endgame function if criterias are met
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetPopulationSum(int newValue)
        {
            // If new value is negative  call endgame function
            if (newValue < 0)
            {
                EndGame(false);
                return false;
            }
            // If we lose population, gain food and water income
            if (newValue < populationSum)
            {
                waterIncrease += populationSum - newValue;
                foodIncrease += populationSum - newValue;
            }
            // If new value is winning value call endgame function
            if (newValue >= PopulationWinningAmount)
            {
                EndGame(true);
                return false;
            }
            populationSum = newValue;
            // If population resource value exceeds resource limit, set to resource limit
            if (populationSum > ResourceValueLimit)
                populationSum = ResourceValueLimit;
            // Change gold income to match new value
            goldIncrease = populationSum / 10;
            return true;
        }

        /// <summary>
        /// Changes used population resource to given value and returns true if criteria are met
        /// Checks if used population resource is less than population sum
        /// Checks if newValue is greater than old value (you cannot decrease used population)
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetPopulationUsedAmount(int newValue)
        {
            if (newValue <= populationUsedAmount)
                return false;
            if (newValue > populationSum)
                return false;
            populationUsedAmount = newValue;
            return true;
        }

        /// <summary>
        /// Changes faith resource to given value
        /// Returns false if new value is less or equal zero or if greater than 100
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool SetFaithAmount(int newValue)
        {
            if (newValue <= 0)
                return false;
            if (newValue > 100)
                return false;
            faithAmount = newValue;
            return true;
        }

        /// <summary>
        /// Updates labelFoodIncrease with formatted value
        /// If value is positive displays + in front of it
        /// </summary>
        private void UpdateLabelFoodIncrease()
        {
            if (foodIncrease > 0)
                labelFoodIncrease.Text = String.Format("+{0}", foodIncrease);
            else
                labelFoodIncrease.Text = foodIncrease.ToString();
        }

        /// <summary>
        /// Updates labelWaterIncrease with formatted value
        /// If value is positive displays + in front of it
        /// </summary>
        private void UpdateLabelWaterIncrease()
        {
            if (waterIncrease > 0)
                labelWaterIncrease.Text = String.Format("+{0}", waterIncrease);
            else
                labelWaterIncrease.Text = waterIncrease.ToString();
        }

        /// <summary>
        /// Updates labelWoodIncrease with formatted value
        /// If value is positive displays + in front of it
        /// </summary>
        private void UpdateLabelWoodIncrease()
        {
            if (woodIncrease > 0)
                labelWoodIncrease.Text = String.Format("+{0}", woodIncrease);
            else
                labelWoodIncrease.Text = woodIncrease.ToString();
        }

        /// <summary>
        /// Updates labelStoneIncrease with formatted value
        /// If value is positive displays + in front of it
        /// </summary>
        private void UpdateLabelStoneIncrease()
        {
            if (stoneIncrease > 0)
                labelStoneIncrease.Text = String.Format("+{0}", stoneIncrease);
            else
                labelStoneIncrease.Text = stoneIncrease.ToString();
        }

        /// <summary>
        /// Updates labelFaithAmount with formatted value
        /// Format is: value%
        /// </summary>
        private void UpdateLabelFaithAmount()
        {
            labelFaithAmount.Text = String.Format("{0}%", faithAmount);
        }

        /// <summary>
        /// Updates labelGoldIncrease with formatted value
        /// Format is: value%
        /// Depends on faith amount
        /// </summary>
        private void UpdateLabelGoldIncrease()
        {
            labelGoldIncrease.Text = String.Format("+{0}", (faithAmount/25) * goldIncrease);
        }

        /// <summary>
        /// Updates labelPopulationIncrease with formatted value
        /// If value is positive displays + in front of it
        /// </summary>
        private void UpdateLabelPopulationIncrease()
        {
            if (populationIncrease > 0)
                labelPopulationIncrease.Text = String.Format("+{0}", populationIncrease);
            else
                labelPopulationIncrease.Text = String.Format("{0}", populationIncrease);
        }

        /// <summary>
        /// Increases food resource increase value per tick
        /// Updates label food increase to match current value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFoodIncrease_Click(object sender, EventArgs e)
        {
            // Store previous wood value in case setting population fails
            int previousWood = woodAmount;
            // Try to subtract 10 from wood amount
            if (SetWoodAmount(woodAmount - 50)) {
                // Try to subtract 1 from population amount
                if (SetPopulationAmount(populationAmount - 1))
                {
                    SetPopulationUsedAmount(populationUsedAmount + 1);
                    foodIncrease += 15;
                    UpdateLabelFoodIncrease();
                    UpdateResourceLabels();
                }
                // If changing population doesn't work, restore wood amount
                else
                    SetWoodAmount(previousWood);
            }
        }

        /// <summary>
        /// Increases water resource increase value per tick
        /// Updates label water increase to match current value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWaterIncrease_Click(object sender, EventArgs e)
        {
            // Store previous wood value in case setting stone fails
            int previousWood = woodAmount;
            // Try to subtract 100 from wood amount
            if (SetWoodAmount(woodAmount - 100))
            {
                // Try to subtract 500 from stone amount
                if (SetStoneAmount(stoneAmount - 500))
                {
                    waterIncrease += 50;
                    UpdateLabelWaterIncrease();
                    UpdateResourceLabels();
                }
                // If changing stone doesn't work, restore wood amount
                else
                    SetWoodAmount(previousWood);
            }
        }

        /// <summary>
        /// Increases wood resource increase value per tick
        /// Updates label wood increase to match current value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWoodIncrease_Click(object sender, EventArgs e)
        {
            // Store previous food value in case setting population fails
            int previousFood = foodAmount;
            // Try to subtract 10 from food amount
            if (SetFoodAmount(foodAmount - 50))
            {
                // Try to subtract 1 from population amount
                if (SetPopulationAmount(populationAmount - 1))
                {
                    SetPopulationUsedAmount(populationUsedAmount + 1);
                    woodIncrease += 10;
                    UpdateLabelWoodIncrease();
                    UpdateResourceLabels();
                }
                // If changing population doesn't work, restore food amount
                else
                    SetFoodAmount(previousFood);
            }
        }

        /// <summary>
        /// Increases stone resource increase value per tick
        /// Updates label stone increase to match current value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStoneIncrease_Click(object sender, EventArgs e)
        {
            // Store previous food value in case setting wood fails
            int previousFood = foodAmount;
            // Try to subtract 100 from food amount
            if (SetFoodAmount(foodAmount - 100))
            {
                // Store previous food value in case setting population fails
                int previousWood = woodAmount;
                // Try to subtract 50 from wood amount
                if (SetWoodAmount(woodAmount - 50))
                {
                    // Try to subtract 2 from population amount
                    if (SetPopulationAmount(populationAmount - 2))
                    {
                        SetPopulationUsedAmount(populationUsedAmount + 2);
                        stoneIncrease += 10;
                        UpdateLabelStoneIncrease();
                        UpdateResourceLabels();
                    }
                    // If changing populations doesn't work, restore wood amount
                    else
                        SetWoodAmount(previousWood);
                }
                // If changing wood doesn't work, restore food amount
                else
                    SetFoodAmount(previousFood);
            }
        }

        /// <summary>
        /// Increases population sum resource
        /// Updates label population to match current value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPopulationIncrease_Click(object sender, EventArgs e)
        {
            // Store previous food value in case setting wood fails
            int previousFood = foodAmount;
            // Try to subtract 500 from food amount
            if (SetFoodAmount(foodAmount - 500))
            {
                // Store previous food value in case setting population fails
                int previousWood = woodAmount;
                // Try to subtract 1000 from wood amount
                if (SetWoodAmount(woodAmount - 1000))
                {
                    // Try to subtract 300 from stone amount
                    if (SetStoneAmount(stoneAmount - 300))
                    {
                        SetPopulationSum(populationSum + 10);
                        // Population update values
                        UpdateResourceLabels();
                    }
                    // If changing populations doesn't work, restore wood amount
                    else
                        SetWoodAmount(previousWood);
                }
                // If changing wood doesn't work, restore food amount
                else
                    SetFoodAmount(previousFood);
            }
        }

        /// <summary>
        /// One time faith upgrade +25 for 100 gold
        /// Disables button
        /// Changes button's color
        /// Tries to increase faith amount by 25
        /// Updates resource and faith labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoldenPulpit_Click(object sender, EventArgs e)
        {
            // Store previous gold value in case setting faith fails
            int previousGold = goldAmount;
            // Try to subtract 100 from gold amount
            if (SetGoldAmount(goldAmount - 100))
            {
                // Try to add 25 to faith amount
                if (SetFaithAmount(faithAmount + 25))
                {
                    buttonGoldenPulpitUpgrade.Enabled = false;
                    buttonGoldenPulpitUpgrade.BackColor = Color.CadetBlue;
                    UpdateLabelFaithAmount();
                    UpdateResourceLabels();
                    UpdateLabelGoldIncrease();
                }
                // If changing faith doesn't work, restore gold amount
                else
                    SetGoldAmount(previousGold);
            }
        }

        /// <summary>
        /// One time faith upgrade +50 for 1000 gold
        /// Disables button
        /// Changes button's color
        /// Tries to increase faith amount by 50
        /// Updates resource and faith labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoldenCrucifixUpgrade_Click(object sender, EventArgs e)
        { 
            // Store previous gold value in case setting faith fails
            int previousGold = goldAmount;
            // Try to subtract 1000 from gold amount
            if (SetGoldAmount(goldAmount - 1000))
            {
                // Try to add 50 to faith amount
                if (SetFaithAmount(faithAmount + 50))
                {
                    buttonGoldenCrucifixUpgrade.Enabled = false;
                    buttonGoldenCrucifixUpgrade.BackColor = Color.CadetBlue;
                    UpdateLabelFaithAmount();
                    UpdateResourceLabels();
                    UpdateLabelGoldIncrease();
                }
                // If changing faith doesn't work, restore gold amount
                else
                    SetGoldAmount(previousGold);
            }
        }

        /// <summary>
        /// One time population upgrade +20 for 200 wood
        /// Disables button
        /// Changes button's color
        /// Tries to increase population sum by 20
        /// Updates resource labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBunkBedsUpgrade_Click(object sender, EventArgs e)
        {
            // Store previous wood value in case setting population fails
            int previousWood = woodAmount;
            // Try to subtract 200 from wood amount
            if (SetWoodAmount(woodAmount - 200))
            {
                // Try to add 20 to population sum
                if (SetPopulationSum(populationSum + 20))
                {
                    buttonBunkBedsUpgrade.Enabled = false;
                    buttonBunkBedsUpgrade.BackColor = Color.CadetBlue;
                    UpdateResourceLabels();
                }
                // If changing population doesn't work, restore wood amount
                else
                    SetWoodAmount(previousWood);
            }
        }

        /// <summary>
        /// One time population upgrade +60 for 500 stone
        /// Disables button
        /// Changes button's color
        /// Tries to increase population sum by 60
        /// Updates resource labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStoneGranaryUpgrade_Click(object sender, EventArgs e)
        {
            // Store previous stone value in case setting population fails
            int previousStone = stoneAmount;
            // Try to subtract 500 from stone amount
            if (SetStoneAmount(stoneAmount - 500))
            {
                // Try to add 60 to population sum
                if (SetPopulationSum(populationSum + 60))
                {
                    buttonStoneGranaryUpgrade.Enabled = false;
                    buttonStoneGranaryUpgrade.BackColor = Color.CadetBlue;
                    UpdateResourceLabels();
                }
                // If changing population doesn't work, restore stone amount
                else
                    SetStoneAmount(previousStone);
            }
        }

        /// <summary>
        /// One time population upgrade +100 for 1000 gold
        /// Disables button
        /// Changes button's color
        /// Tries to increase population sum by 100
        /// Updates resource labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSpicesUpgrade_Click(object sender, EventArgs e)
        {
            // Store previous gold value in case setting population fails
            int previousGold = goldAmount;
            // Try to subtract 1000 from gold amount
            if (SetGoldAmount(goldAmount - 1000))
            {
                // Try to add 100 to population sum
                if (SetPopulationSum(populationSum + 100))
                {
                    buttonSpicesUpgrade.Enabled = false;
                    buttonSpicesUpgrade.BackColor = Color.CadetBlue;
                    UpdateResourceLabels();
                }
            }
            // If changing population doesn't work, restore gold amount
            else
                SetGoldAmount(previousGold);
        }

        /// <summary>
        /// One time hunting upgrade for 300 food
        /// Disables button
        /// Changes button's color
        /// Changes wolves attack event behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHuntingEventUpgrade_Click(object sender, EventArgs e)
        {
            // Try to subtract 300 from food amount
            if (SetFoodAmount(foodAmount - 300))
            {
                buttonHuntingEventUpgrade.Enabled = false;
                buttonHuntingEventUpgrade.BackColor = Color.Wheat;
                huntingUpgradeActive = true;
                UpdateResourceLabels();
            }
        }

        /// <summary>
        /// One time irrigation upgrade for 450 wood
        /// Disables button
        /// Changes button's color
        /// Changes rain event behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonIrrigationEventUpgrade_Click(object sender, EventArgs e)
        {
            // Try to subtract 450 from wood amount
            if (SetWoodAmount(woodAmount - 450))
            {
                buttonIrrigationEventUpgrade.Enabled = false;
                buttonIrrigationEventUpgrade.BackColor = Color.Wheat;
                irrigationUpgradeActive = true;
                UpdateResourceLabels();
            }
        }

        /// <summary>
        /// One time walls upgrade for 200 wood and 400 stone
        /// Disables button
        /// Changes button's color
        /// Changes bandits attack event behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWallsEventUpgrade_Click(object sender, EventArgs e)
        {
            // Stores previous wood value in case setting stone fails
            int previousWood = woodAmount;
            // Try to subtract 200 from wood amount
            if (SetWoodAmount(woodAmount - 200))
            {
                // Try to subtract 400 from stone amount
                if (SetStoneAmount(stoneAmount - 400))
                {
                    buttonWallsEventUpgrade.Enabled = false;
                    buttonWallsEventUpgrade.BackColor = Color.Wheat;
                    wallsUpgradeActive = true;
                    UpdateResourceLabels();
                }
                // If changing stone doesn't work, restore wood amount
                else
                    SetWoodAmount(previousWood);
            }
        }

        /// <summary>
        /// One time navigation upgrade for 150 gold
        /// Disables button
        /// Changes button's color
        /// Changes ship drowned event behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNavigationEventUpgrade_Click(object sender, EventArgs e)
        {
            // Try to subtract 150 from gold amount
            if (SetGoldAmount(goldAmount - 150))
            {
                buttonNavigationEventUpgrade.Enabled = false;
                buttonNavigationEventUpgrade.BackColor = Color.Wheat;
                navigationUpgradeActive = true;
                UpdateResourceLabels();
            }
        }

        /// <summary>
        /// One time wells upgrade for 200 wood and 400 stone
        /// Disables button
        /// Changes button's color
        /// Changes fire event behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWellsEventUpgrade_Click(object sender, EventArgs e)
        {
            // Stores previous wood value in case setting stone fails
            int previousWood = woodAmount;
            // Try to subtract 200 from wood amount
            if (SetWoodAmount(woodAmount - 200))
            {
                // Try to subtract 400 from stone amount
                if (SetStoneAmount(stoneAmount - 400))
                {
                    buttonWellsEventUpgrade.Enabled = false;
                    buttonWellsEventUpgrade.BackColor = Color.Wheat;
                    wellsUpgradeActive = true;
                    UpdateResourceLabels();
                }
                // If changing stone doesn't work, restore wood amount
                else
                    SetWoodAmount(previousWood);
            }
        }

        /// <summary>
        /// One time medicine upgrade for 600 food and 400 gold
        /// Disables button
        /// Changes button's color
        /// Changes disease event behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMedicineEventUpgrade_Click(object sender, EventArgs e)
        {
            // Stores previous food value in case setting gold fails
            int previousFood = foodAmount;
            // Try to subtract 600 from food amount
            if (SetFoodAmount(foodAmount - 600))
            {
                // Try to subtract 400 from gold amount
                if (SetGoldAmount(goldAmount - 400))
                {
                    buttonMedicineEventUpgrade.Enabled = false;
                    buttonMedicineEventUpgrade.BackColor = Color.Wheat;
                    medicineUpgradeActive = true;
                    UpdateResourceLabels();
                }
                // If changing gold doesn't work, restore food amount
                else
                    SetFoodAmount(previousFood);
            }
        }

        /// <summary>
        /// Temporary cheat function for developer to test
        /// Gives resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelFaithPlaceholder_Click(object sender, EventArgs e)
        {
            woodAmount += 500;
            stoneAmount += 500;
            waterAmount += 500;
            populationSum += 30;
            populationAmount += 30;
            foodAmount += 500;
            goldAmount += 500;
            UpdateResourceLabels();
        }

        /// <summary>
       /// Enables all game speed buttons
       /// Changes all game speed buttons borders to none
       /// </summary>
        private void EnableGameSpeedButtons()
        {
            pictureBoxPauseGame.Enabled = true;
            pictureBoxPauseGame.BorderStyle = BorderStyle.None;
            pictureBoxSlowGameSpeed.Enabled = true;
            pictureBoxSlowGameSpeed.BorderStyle = BorderStyle.None;
            pictureBoxNormalGameSpeed.Enabled = true;
            pictureBoxNormalGameSpeed.BorderStyle = BorderStyle.None;
            pictureBoxFastGameSpeed.Enabled = true;
            pictureBoxFastGameSpeed.BorderStyle = BorderStyle.None;
        }

        /// <summary>
        /// Stops game until other game speed button is clicked
        /// Disables button
        /// Changes button's color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPauseGame_Click(object sender, EventArgs e)
        {
            EnableGameSpeedButtons();
            pictureBoxPauseGame.Enabled = false;
            pictureBoxPauseGame.BorderStyle = BorderStyle.FixedSingle;
            timerGameSpeed.Stop();
        }

        /// <summary>
        /// Changes game speed to slow until other game speed button is clicked
        /// Disables button
        /// Changes button's color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxSlowGameSpeed_Click(object sender, EventArgs e)
        {
            EnableGameSpeedButtons();
            pictureBoxSlowGameSpeed.Enabled = false;
            pictureBoxSlowGameSpeed.BorderStyle = BorderStyle.FixedSingle;
            timerGameSpeed.Interval = SlowGameSpeed;
            timerGameSpeed.Start();
        }

        /// <summary>
        /// Changes game speed to normal until other game speed button is clicked
        /// Disables button
        /// Changes button's color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxNormalGameSpeed_Click(object sender, EventArgs e)
        {
            EnableGameSpeedButtons();
            pictureBoxNormalGameSpeed.Enabled = false;
            pictureBoxNormalGameSpeed.BorderStyle = BorderStyle.FixedSingle;
            timerGameSpeed.Interval = NormalGameSpeed;
            timerGameSpeed.Start();
        }

        /// <summary>
        /// Changes game speed to fast until other game speed button is clicked
        /// Disables button
        /// Changes button's color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxFastGameSpeed_Click(object sender, EventArgs e)
        {
            EnableGameSpeedButtons();
            pictureBoxFastGameSpeed.Enabled = false;
            pictureBoxFastGameSpeed.BorderStyle = BorderStyle.FixedSingle;
            timerGameSpeed.Interval = FastGameSpeed;
            timerGameSpeed.Start();
        }
    }
}
