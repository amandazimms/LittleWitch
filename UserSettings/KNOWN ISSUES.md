# Known Issues

## Important
- [ ] When you're helping one peasant, take it out of the queue of peasants so others go 'past' it.
- [ ] see todos around lines 140-160 of PeasantStats (end of GivePotion method) - need to add more feedback for user after potions are taken
- [ ] "NEW GAME" button after winning does nothing: also test other buttons
- [ ] fancy-first-initial isn't placing itself well (with different screen sizes, or different title lengths) - disable this and move to stretch goals


## Not Important
-main menu titles (probably other menu titles too) change size drastically in fullscreen v small
-Peasant hair doesn't bounce when walking (sickness anim takes precedence)
-Animate the sun's Y position with sunrise and sunset, but its X position should be tied to the camera / parallaxed
-tweak peasant sizes - some are tiny
-fun to randomize "Ah, Morning" on the popup to different texts


Week 3 (9/27): Popup system: introduce and end (win or lose) game, plus one for each day. Also add special guests and popups for these.

Morning popup (and all pop ups) should pause the game 

Score not showing

	Monday: Build popup UI - quick, inspired by FF, and some existing in LW. Start on programming if time.
	(No Tuesday)
	Wednesday: Finish programming - popup manager that listens to day counter for which popup to show. Start with a basic outline: "day 3: 6 peasants". For this MVP, every day is predetermined - e.g. special guests ARE on day 6 and 9, not chance of one between those dates.
	(No Thursday):
	Friday: Add game start, end: win and end: lose popups
	Saturday morning: Brainstorm: rather than day 1,2,3; 15,14,13 days until X event might be more interesting? Brainstorm story.
	Sunday morning: Polish and catch-up - make gifs for IG

Week 4 (10/4): Explore UX of what-cures-what: a book that shows the elemental relations? Popups that explain more? Build out "diagnose" or "chat about symptoms" feature. 
	
	Monday: Work on chat feature - add basic animation and program to hook it up.
	Tuesday?:
	Wednesday:
	Thursday?:
	Friday:
	Saturday: Polish and catch-up - make gifs for IG
	Sunday: Polish and catch-up - make gifs for IG

Week 5 (10/11): Polish, save game?

	Monday: 
	Tuesday?
	Wednesday:
	Thursday?:
	Friday:
	Saturday: Polish and catch-up - make gifs for IG
	Sunday: Polish and catch-up - make gifs for IG



Known issue: drips particles are finicky: may appear on builds, but not in game window (unless after being paused / navigating to scene view). MoveAlongPath script may have something to do with this.

Known issue: on smaller screen resolutions, the positioning of the PotionNumber and PotionIcon in the game overlay appear offset.

Known issue: when selecting LW, there are no menu options set, so the selection menu is populated with the previous selection's options.

Known issue: when trying to 'cancel' an action by, for example, right clicking to move, the action still proceeds. Check FoxFable for what we did there.