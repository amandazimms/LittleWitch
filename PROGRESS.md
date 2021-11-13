# Progress and Planning Tracker


## Game Popups System
*Gameplay is separated into days: during the night, peasants come and must be cured. At the end of night is dawn, during which the player can see the sun rising (and time running out). At daybreak, any remaining peasants will leave and damage the reputation meter. During day, LW can brew potions until night falls again.*

*The popups system is how the player feels the game progressing - each day at daybreak stats from that night are displayed, and info about the following night (a "forecast") is shown/hinted at so LW can prepare*

- [ ] Decision: rather than just surviving for 14 days, "14 days until X event" could be more interesting? Brainstorm story ideas for the event
  - [ ] Implement this storyline into the daily popups


## Elemental Knowledge System
*Potions in LW are elemental: water cures fire, fire cures air, air cures earth, earth cures water. (In stretch goals, combinations of these elements also have rock-paper-scissors like relationships, such as ice curing mud.)*

- [ ] Decision / Exploration: UX of figuring out and tracking 'what cures what'. A book that shows the elemental relations? Popups that explain more? 

- [ ] Build out "diagnose" or "chat about symptoms" feature:
  - [ ] When a peasant is selected, show this option. A popup could display text of what the peasant said, e.g. "You've got to help me! I can't stop burping these red, glowy, flamey things!" 
  - [ ] Experiment with showing a 'hint' button on this popup ^, which could reveal a fire icon in the popup. Then decide if the popup would always be the same for each interaction on peasants with the same symptoms, and once the hint is revealed, is it always revealed? 
  - [ ] Need animations for the peasant and witch chatting

- [ ] Need more immediate feedback for when a potion is taken - particle systems to show positive/negative results, animation that draws the eye to * tally at the top and/or the number turns red/green as it changes?


## Special Guest System
*To keep things exciting, special guests will occasionally visit LW along with the townsfolk. Curing them successfully will earn more points than a normal peasant, and giving the wrong cure loses more*

- [ ] Visuals:
  - [ ] Add particle system to make them glow/shine etc.
  - [ ] Change sprites and or color to make them stand out

- [ ] Logic:
  - [ ] Add a bool for special guest in Peasant Stats. If this is checked when they're cured, increase the 'numCured' by a greater amount - 1.5, 2?


## Saving Progress

- [ ] Build out UI for this:
 - [ ] Settings icon that opens Save/Quit menu	(&/or escape key)
 - [ ] Clone Main Menu popup and use that, or tweak existing one so it can be used for either

- [ ] Logic: 
  - [ ] Data that needs to be stored:
    - [ ] Game state:
      - [ ] Plant, potion, and score tracker counts
      - [ ] Reputation meter status
      - [ ] Day state and countdown, and dayCount
      - [ ] Selected - what is currently selected
      - [ ] Hut state - inside or outside
      - [ ] In startup menu or in gameplay
    - [ ] Each peasant on the screen:
      - [ ] Position & scale
      - [ ] Anim state?
      - [ ] Condition (sickness)
      - [ ] currentWaitTime
      - [ ] peasantInFrontOfMe
    - [ ] LW:
      - [ ] Position & scale
      - [ ] Carried item
      - [ ] Inside Hut bool
    - [ ] Cauldron:
      - [ ] Hourglass spinner stats
      - [ ] Brew stage
      - [ ] Num potions
      - [ ] All colors
      - [ ] Current ingredients
    - [ ] Variance scripts data - could create a way to loop through every GO in scene and check for these scripts on all child GOs? Then store data and which GO it's on?
    - [ ] Camera position & zoom 

- [ ] Way to access saved game from UI (only one save slot for now)



## Stretch Goals
*Above is the MVP! Here are some more fun 'someday' thoughts*

#### Tier two:
    Brew your own: Endless ingredients, but must experiment with mixing them together to make new potions 
    Can brew anytime, but best for gameplay to do during day as peasants come at night only.
    As days go on, your owl* drops off new ingredients between days - this story reveal only via text pop ups. Friendly peasants could give ingredients too.
      *the owl is just a static sprite you learn about in the popups, not interactable
    Each morning you fill out a dialog asking the owl which ingredients to bring: she can only carry so many 

#### Tier three: 
   Grow your own: now the owl brings seeds instead. 
   Gameplay would involve a lot of switching between garden and cauldron when peasants not there.
   Garden space to the right of the house 
   No variation within type of plant, and simplify (maybe recolor same sprites to upgrade to “super”___ or “toxic”____  version)
   Force specific amount of time to do gardening - until sundown when TD aspect begins 


#### Tier four:
    The owl is an actual character you interact with - she hoots when she has new seed and you click her each day to unpackage her parcels
    Cats. Maybe different cats (colors) come by each day and you can feed them catnip to make them stick around? Once you have a cat you have to feed it to keep it around, but it will bring you goodies/seeds/etc.
    Peasant color variance should have types of hair-skin color combos that look more realistic
    Add a collectible element - could be things that the cat brings, some that peasants/special guests give you, or some that you find 'digging' (while gardening)
    Achievements, especially to grow all plants and brew all potions
    Advanced potions could require non-plant elements: crystals/ores, or animal products. To simplify, LW could have a blacksmith/milkmaid friend who brings these if ___


   
 
