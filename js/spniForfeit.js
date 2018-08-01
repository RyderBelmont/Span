/********************************************************************************
 This file contains the variables and functions that allow for players to have
 multiple potential forfeits.
 ********************************************************************************/
 
/**********************************************************************
 *****                 Forfeit Object Specification               *****
 **********************************************************************/

var CAN_SPEAK = true;
var CANNOT_SPEAK = false;
 
/**********************************************************************
 *****                      Forfeit Variables                     *****
 **********************************************************************/
 
/* orgasm timer */
var ORGASM_DELAY = 2000;

/* The first and last rounds a character starts heavy masturbation, counted in phases before they finish */
var HEAVY_FIRST_ROUND = 6;
var HEAVY_LAST_ROUND = 2;
 
/* forfeit timers */
var timers = [0, 0, 0, 0, 0];
 
/**********************************************************************
 *****                      Forfeit Functions                     *****
 **********************************************************************/

/************************************************************
 * Sets the forfeit timer of the given player, with a random
 * offset, if applicable.
 ************************************************************/
function setForfeitTimer (player) {
	// THE TIMER IS HARD SET RIGHT NOW
	timers[player] = players[player].timer;
    if (player == HUMAN_PLAYER) {
      $gamePlayerCountdown.html(timers[player]);
    }
	
	// THE STAGE IS HARD SET RIGHT NOW
	players[player].stage += 1;
	players[player].timeInStage = -1;
	players[player].updateLabel();
}

/************************************************************
 * Initiate masturbation for the selected player
 ************************************************************/
function startMasturbation (player) {
	players[player].forfeit = [PLAYER_MASTURBATING, CAN_SPEAK];
	players[player].out = true;

    if (chosenDebug === player) {
        chosenDebug = -1;
        updateDebugState(showDebug);
    }

	/* update behaviour */
	if (player == HUMAN_PLAYER) {
		if (players[HUMAN_PLAYER].gender == eGender.MALE) {
			updateAllBehaviours(HUMAN_PLAYER, MALE_START_MASTURBATING, players[HUMAN_PLAYER]);
		} else if (players[HUMAN_PLAYER].gender == eGender.FEMALE) {
			updateAllBehaviours(HUMAN_PLAYER, FEMALE_START_MASTURBATING, players[HUMAN_PLAYER]);
		}
		$gameClothingLabel.html("You're Masturbating...");
        $gamePlayerCountdown.show();
		setForfeitTimer(player);
	} else {
		if (players[player].gender == eGender.MALE) {
			updateAllBehaviours(player, MALE_START_MASTURBATING, players[player]);
		} else if (players[player].gender == eGender.FEMALE) {
			updateAllBehaviours(player, FEMALE_START_MASTURBATING, players[player]);
		}
		updateBehaviour(player, PLAYER_START_MASTURBATING);
		setForfeitTimer(player);
	}
	updateAllGameVisuals();

	/* allow progression */
	endRound();
}

/************************************************************
 * The forfeit timers of all players tick down, if they have 
 * been set.
 ************************************************************/
function tickForfeitTimers () {
    console.log("Ticking forfeit timers...");
    
    var masturbatingPlayers = [];

	for (var i = 0; i < players.length; i++) {
		if (players[i] && players[i].out && !players[i].finished && timers[i] == 0) {
			finishMasturbation(i);
			allowProgression();
			return true;
		}
	}

    if (gamePhase != eGamePhase.STRIP) for (var i = 0; i < players.length; i++) {
        if (players[i] && players[i].out && timers[i] == 1) {
            timers[i] = 0;
            /* set the button state */
            $mainButton.html("Cumming...");
            $mainButton.attr('disabled', true);
            actualMainButtonState = true;

            if (i == HUMAN_PLAYER) {
                /* player's timer is up */
                $gamePlayerCountdown.hide();
                console.log(players[i].label+" is finishing!");
                $gameClothingLabel.html("<b>You're 'Finished'</b>");

                /* finish */
                finishMasturbation(i);
            } else {
                console.log(players[i].label+" is finishing!");

                /* hide everyone else's dialogue bubble */
                for (var j = 1; j < players.length; j++) {
                    if (i != j) {
                        $gameDialogues[j-1].html("");
                        $gameAdvanceButtons[j-1].css({opacity : 0});
                        $gameBubbles[j-1].hide();
                    }
                }

                /* let the player speak again */
                players[i].forfeit = [PLAYER_FINISHING_MASTURBATING, CAN_SPEAK];

                /* show them cumming */
                updateBehaviour(i, PLAYER_FINISHING_MASTURBATING);
                updateGameVisual(i);

                /* trigger the callback */
                var player = i, tableVisible = (tableOpacity > 0);
                timeoutID = window.setTimeout(function(){ allowProgression(eGamePhase.END_FORFEIT); }, ORGASM_DELAY);
				globalSavedTableVisibility = tableVisible;
                if (AUTO_FADE) forceTableVisibility(false);
            }
            return true;
        }
    }

    for (var i = 0; i < players.length; i++) {
		if (players[i] && players[i].out && timers[i] > 1) {
        	timers[i]--;

			if (i == HUMAN_PLAYER) {
				/* human player */
				/* update the player label */
				$gameClothingLabel.html("<b>'Finished' in "+timers[i]+" phases</b>");
				$gamePlayerCountdown.html(timers[i]);	
				masturbatingPlayers.push(i); // Double the chance of commenting on human player
			} else {
				/* AI player */
				/* random chance they go into heavy masturbation */
				// CHANGE THIS TO ACTIVATE ONLY IN THE LAST 4 TURNS
				var randomChance = getRandomNumber(HEAVY_LAST_ROUND, HEAVY_FIRST_ROUND);
				
				if (randomChance > timers[i]-1) {
					/* this player is now heavily masturbating */
					players[i].forfeit = [PLAYER_HEAVY_MASTURBATING, CANNOT_SPEAK];
					updateBehaviour(i, PLAYER_HEAVY_MASTURBATING);
					updateGameVisual(i);
				}
			}
			masturbatingPlayers.push(i);
        }
    }
	
	// Show a player masturbating while dealing or after the game, if there is one available
	if (masturbatingPlayers.length > 0
		&& ((gamePhase == eGamePhase.DEAL && players[HUMAN_PLAYER].out) || gamePhase == eGamePhase.EXCHANGE || gamePhase == eGamePhase.END_LOOP)) {
		var playerToShow = masturbatingPlayers[getRandomNumber(0, masturbatingPlayers.length)];//index of player chosen to show masturbating//players[]
		for (var i = 1; i < players.length; i++) {
			updateBehaviour(i,
							(i == playerToShow) ? players[i].forfeit[0]
							: (players[playerToShow].gender == eGender.MALE ? MALE_MASTURBATING : FEMALE_MASTURBATING),
							players[playerToShow]);
		}
		updateAllGameVisuals();
	}
	
	return false;
}

/************************************************************
 * A player has 'finished' masturbating.
 ************************************************************/
function finishMasturbation (player) {
	// HARD SET STAGE
	players[player].stage += 1;
	players[player].timeInStage = -1;
	players[player].finished = true;
    players[player].forfeit = [PLAYER_FINISHED_MASTURBATING, CAN_SPEAK];
	players[player].updateLabel();

	/* update other player dialogue */
	if (players[player].gender == eGender.MALE) {
		updateAllBehaviours(player, MALE_FINISHED_MASTURBATING, players[player]);
	} else if (players[player].gender == eGender.FEMALE) {
		updateAllBehaviours(player, FEMALE_FINISHED_MASTURBATING, players[player]);
	}
	
	/* update their dialogue */
	if (player != HUMAN_PLAYER) {
		updateBehaviour(player, PLAYER_FINISHED_MASTURBATING);
		
	}
	updateAllGameVisuals();
	if (AUTO_FADE && globalSavedTableVisibility !== undefined) {
		forceTableVisibility(globalSavedTableVisibility);
		globalSavedTableVisibility = undefined;
	}
	allowProgression();
}
