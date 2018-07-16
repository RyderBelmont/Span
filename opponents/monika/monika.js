if(!monika) {
    var monika = {};

    console.log("[Monika] Executing monika.js...");

    /* Base probabilities for glitch effects */
    monika.CHARACTER_CONT_GLITCH_CHANCE = 0.05;
    monika.CHARACTER_GLITCH_CHANCE = 0.05;
    monika.DIALOGUE_GLITCH_CHANCE = 0.05;
    monika.EDIT_GLITCH_CHANCE = 0.05;
    monika.DELETE_GLITCH_CHANCE = 0.05;
    
    monika.GLITCH_VISUAL = 'visual';  // continuous visual glitching
    monika.GLITCH_REPEAT = 'corrupt'; // dialogue corruption
    monika.GLITCH_EDIT = 'edit';      // edited dialogue
    monika.GLITCH_DELETE = 'delete';  // fake-deleted character

    monika.incognito = false;

    /* Check for incognito / private browsing mode. */
    /* Should work for Chrome and Firefox. */
    var fs = window.RequestFileSystem || window.webkitRequestFileSystem;
    if (!fs) {
        var db = indexedDB.open("test");
        db.onerror = function () { monika.incognito = true; console.log("[Monika] Detected Firefox Private Browsing mode."); }
        db.onsuccess = function () {
            using_incognito_mode = false;

            /* Test for Safari private mode */
            var storage = window.sessionStorage;
            try {
                storage.setItem("test_key", "test");
                storage.removeItem("test_key");
            } catch (e) {
                if (e.code === DOMException.QUOTA_EXCEEDED_ERR && storage.length === 0) {
                    console.log("[Monika] Detected Safari Private Browsing mode.");
                    monika.incognito = true;
                }
            }
        }
    } else {
        fs(window.TEMPORARY, 100, function(fs) {
            monika.incognito = false;
        }, function(err) {
            console.log("[Monika] Detected Chrome Incognito mode.");
            monika.incognito = true;
        });
    }

    monika.loadScript = function (scriptName) {
        console.log("[Monika] Loading module: "+scriptName);
        return $.getScript(scriptName).fail(function( jqxhr, settings, exception ) {
            console.error("[Monika] Error loading "+scriptName+": \n"+exception.toString());
        })
    }

    monika.loadScript("opponents/monika/js/canvas_utils.js");
    monika.loadScript("opponents/monika/js/effects.js");
    monika.loadScript("opponents/monika/js/glitches.js");
    monika.loadScript("opponents/monika/js/behaviour_callbacks.js");
    monika.loadScript("opponents/monika/js/extended_dialogue.js");

    /* Load CSS: */
    $('head').append('<link rel="stylesheet" type="text/css" href="opponents/monika/css/monika.css">');

    monika.extract_oppID = function(folder) {
        var oppID = folder.substr(0, folder.length - 1);
        return oppID.substr(oppID.lastIndexOf("/") + 1).toLowerCase();
    }

    monika.find_slot_by_id = function(id) {
        var lowercase_id = id.toLowerCase();
        
        for(var i=0;i<players.length;i++) {
            if(players[i]) {
                var oppID = monika.extract_oppID(players[i].folder);
                
                if(oppID === lowercase_id) {
                    return i;
                }
            }
        }
    }

    monika.find_slot = function () {
        return monika.find_slot_by_id('monika');
    }

    monika.find_monika_player = function() {
        var slot = monika.find_slot();
        if(slot) {
            return players[slot];
        }
    }
    
    monika.present = function() {
        return monika.find_slot() != undefined;
    }
    
    /* pause the main game-- ensures effects aren't skipped by accident
     * Call allowProgression() to reverse this.
     */
    monika.disable_progression = function () {
        $mainButton.attr('disabled', true);
        actualMainButtonState = true;
    }
    
    monika.get_random_player = function(include_monika, include_glitched) {
        var activeSlots = [];
        for(var i=1;i<players.length;i++) {
            if(players[i]) {
                if(include_monika || monika.extract_oppID(players[i].folder) !== 'monika') {
                    if(include_glitched || !monika.active_effects.character_glitching[i]) {
                        activeSlots.push(i);
                    }
                }
            }
        }

        return activeSlots[getRandomNumber(0, activeSlots.length)];
    }
    
    monika.getGlitchMultiplier = function() {        
        // allow glitch chance multiplier to be set manually for debugging purposes
        if(monika.glitch_chance_mult) {
            var glitch_chance_mult = monika.glitch_chance_mult;
        } else {
            var pl = monika.find_monika_player();
            var glitch_chance_mult = 0;
            
            if (pl.stage < 3) {
                glitch_chance_mult = 0;
            } else if (pl.stage === 3) {
                glitch_chance_mult = 0.5;
            } else if (pl.stage <= 5) {
                glitch_chance_mult = 1;
            } else if (pl.stage <= 7) {
                glitch_chance_mult = 1.5;
            } else {
                glitch_chance_mult = 2;
            }
        }
        
        console.log("[Monika] Current glitch chance multiplier: "+glitch_chance_mult.toString());
        
        return glitch_chance_mult;
    }
    
    monika.modifiedChance = function(base_chance, chance_multiplier) {
        var actual_chance = base_chance * chance_multiplier;
        actual_chance = Math.max(0, Math.min(1, actual_chance)); // clip to [0, 1];
        
        return Math.random() <= actual_chance;
    }
    
    monika.setGlitchingMarker = function(targetedSlot, glitch_type, value) {
        var monika_player = monika.find_monika_player();
        var targeted_player = players[targetedSlot];
        
        var oppID = targeted_player.folder.substr(0, targeted_player.folder.length - 1);
        oppID = oppID.substr(oppID.lastIndexOf("/") + 1);
        
        var type_glitch_marker = 'glitch-type-'+glitch_type;
        var base_glitch_marker = 'glitching-'+oppID;
        var specific_glitch_marker = base_glitch_marker+'-'+glitch_type;
        
        if(value) {
            monika_player.markers[base_glitch_marker] = value;
            monika_player.markers[specific_glitch_marker] = value;
            monika_player.markers[type_glitch_marker] = value;
            monika_player.markers['glitched'] = true;
        } else {
            delete monika_player.markers[base_glitch_marker];
            delete monika_player.markers[specific_glitch_marker];
            delete monika_player.markers[type_glitch_marker];
        }
    }
    
    monika.setRoundGlitchMarker = function(value) {
        var monika_player = monika.find_monika_player();
        if(value) {
            monika_player.markers['round-glitched'] = true;
        } else {
            delete monika_player.markers['round-glitched'];
        }
    }
    
    monika.onRoundStart = function() {
        var glitch_chance_mult = monika.getGlitchMultiplier();
        
        /* Clean up any lingering effects from e.g. dialogue scripting */
        monika.cleanUpEffects();
        
        monika.setRoundGlitchMarker(false);
        
        if(!monika.effects_enabled()) {
            return; // bail now if effects are disabled
        }
        
        if(monika.force_cont_glitching || monika.modifiedChance(monika.CHARACTER_CONT_GLITCH_CHANCE, glitch_chance_mult)) {
            /* Pick a character to start glitching continuously. */
            if(monika.force_cont_glitching) {
                var targetedSlot = monika.force_cont_glitching;
            } else {
                var targetedSlot = monika.get_random_player(false, true);
            }
            
            console.log("[Monika] Targeted slot "+targetedSlot.toString()+" for round continuous glitching...");
            
            monika.setGlitchingMarker(targetedSlot, monika.GLITCH_VISUAL, true);
            monika.active_effects.round_targeted_glitching = targetedSlot;
            
            monika.setRoundGlitchMarker(true);
        }
        
        if(monika.force_delete_glitch || monika.modifiedChance(monika.DELETE_GLITCH_CHANCE, glitch_chance_mult)) {
            console.log("[Monika] Performing 'delete' glitch...");
            
            if(monika.force_delete_glitch) {
                var targetedSlot = monika.force_delete_glitch;
            } else {
                var targetedSlot = monika.get_random_player(false, false);
            }
            
            var original_label = players[targetedSlot].label;
            monika.corruptCharacterLabel(targetedSlot);
            
            var cv = monika.get_empty_canvas($gameImages[targetedSlot-1]);
            monika.tile_filter(cv, $gameImages);
            
            monika.active_effects.round_delete_glitching = {
                'slot': targetedSlot,
                'cv': cv,
                'original_label': original_label
            };
            
            monika.setGlitchingMarker(targetedSlot, monika.GLITCH_DELETE, true);
            monika.setRoundGlitchMarker(true);
        }
        
        if(monika.force_dialogue_glitch || monika.modifiedChance(monika.DIALOGUE_GLITCH_CHANCE, glitch_chance_mult)) {
            console.log("[Monika] Glitching a character's dialogue...");
            
            if(monika.force_dialogue_glitch) {
                var targetedSlot = monika.force_dialogue_glitch;
            } else {
                var targetedSlot = monika.get_random_player(false, true);
            }
            
            monika.active_effects.round_dialogue_glitching = targetedSlot;
            monika.setGlitchingMarker(targetedSlot, monika.GLITCH_REPEAT, true);
            monika.setRoundGlitchMarker(true);
        }
        
        if(monika.force_edit_glitch || monika.modifiedChance(monika.EDIT_GLITCH_CHANCE, glitch_chance_mult)) {
            console.log("[Monika] Performing 'edit' glitch...");
            
            if(monika.force_edit_glitch) {
                var targetedSlot = monika.force_edit_glitch;
            } else {
                var targetedSlot = monika.get_random_player(false, true);
            }
            
            monika.active_effects.round_edit_glitching = targetedSlot;
            monika.setGlitchingMarker(targetedSlot, monika.GLITCH_EDIT, true);
            monika.setRoundGlitchMarker(true);
        }
    }

    monika.onPlayerTurn = function () {
        if(!monika.effects_enabled()) {
            return; // bail now if effects are disabled
        }
        
        console.log("[Monika] It's now the player's turn!");
        
        var glitch_chance_mult = monika.getGlitchMultiplier();

        if(monika.modifiedChance(monika.CHARACTER_GLITCH_CHANCE, glitch_chance_mult)) {
            console.log("[Monika] Glitching a random character's images...");
            
            var targetedSlot = monika.get_random_player(true, false);
            monika.temporaryCharacterGlitch(targetedSlot, getRandomNumber(500, 1500), 750);
            
            if(monika.modifiedChance(monika.CHARACTER_GLITCH_CHANCE / 3, glitch_chance_mult)) {
                console.log("[Monika] Glitching an adjacent character as well...");
                
                var adjSlot = targetedSlot;
                if(Math.random() <= 0.5) {
                    adjSlot++;
                } else {
                    adjSlot--;
                }
                
                if(adjSlot <= 0) {
                    adjSlot = players.length-1;
                } else if(adjSlot >= players.length) {
                    adjSlot = 1;
                }
                
                monika.temporaryCharacterGlitch(adjSlot, getRandomNumber(500, 1500), 750);
            }
        }
    }

    monika.onMonikaTurn = function () {
        console.log("[Monika] It's Monika's turn!");
    }
    
    /* monkey patch advanceGame() so we can intercept main button presses */
    var original_advanceGame = advanceGame;
    advanceGame = function() {
        if(!monika.present()) { return original_advanceGame.apply(null, arguments); }
        
        /* we don't use a finally... clause here, because if we're using a custom
         * context then we don't want the original function to be called.
         */
        try {
            if(monika.current_ext_dialogue) {
                monika.extended_dialogue_continue();
            } else {
                original_advanceGame.apply(null, arguments);
            }
        } catch(e) {
            console.error("[Monika] Error in pre-advanceGame prep: "+e.toString());
            original_advanceGame.apply(null, arguments);
        }
    }

    /* monkey patch advanceTurn() so we can do spooky stuff */
    var original_advanceTurn = advanceTurn;
    advanceTurn = function() {
        if(!monika.present()) {
            /* if Monika isn't in this game then strictly do normal behaviour */
            return original_advanceTurn.apply(null, arguments);
        }
        
        try {
            var active_slot = currentTurn+1;
            if(active_slot >= players.length) {
                active_slot = 0;
            }
            
            if(currentTurn === 0) {
                monika.onRoundStart();
            }
        } catch (e) {
            console.error("[Monika] Error in pre-advanceTurn prep: "+e.toString());
        } finally {
            original_advanceTurn();
        }
        
        if(active_slot != undefined && players[active_slot]) {
            if(players[active_slot].folder === 'opponents/human/') {
                monika.onPlayerTurn();
            } else if(players[active_slot].folder === 'opponents/monika/') {
                monika.onMonikaTurn();
            } else if(active_slot === monika.active_effects.round_targeted_glitching) {
                monika.startCharacterGlitching(active_slot, 750, 750);
            }
        }
    }
    
    monika.undoDeleteGlitchEffect = function() {
        var deleteGlitchInfo = monika.active_effects.round_delete_glitching;
        if(deleteGlitchInfo) {
            monika.setGlitchingMarker(deleteGlitchInfo.slot, monika.GLITCH_DELETE, null);
            monika.undoLabelCorruption(deleteGlitchInfo.slot);
        }
        
        monika.active_effects.round_delete_glitching = null;
    }
    
    /* hook into completeContinuePhase to undo deletion glitches */
    var original_completeContinuePhase = completeContinuePhase;
    completeContinuePhase = function() {
        if(!monika.present()) { return original_completeContinuePhase.apply(null, arguments); }
            
        try {
            monika.undoDeleteGlitchEffect();
        } catch (e) {
            console.error("[Monika] Error in pre-completeContinuePhase prep: "+e.toString());
        } finally {
            return original_completeContinuePhase.apply(null, arguments); 
        }
    }
    
    var original_completeMasturbatePhase = completeMasturbatePhase;
    completeMasturbatePhase = function() {
        if(!monika.present()) { return original_completeMasturbatePhase.apply(null, arguments); }
            
        try {
            monika.undoDeleteGlitchEffect();
        } catch (e) {
            console.error("[Monika] Error in pre-completeMasturbatePhase prep: "+e.toString());
        } finally {
            return original_completeMasturbatePhase.apply(null, arguments); 
        }
    }
    
    /* hook into completeStripPhase to undo deletion glitches when the player loses the round */
    var original_completeStripPhase = completeStripPhase;
    completeStripPhase = function() {
        if(!monika.present()) { return original_completeStripPhase.apply(null, arguments); }
        
        try {
            monika.undoDeleteGlitchEffect();
        } catch (e) {
            console.error("[Monika] Error in pre-completeStripPhase prep: "+e.toString());
        } finally {
            return original_completeStripPhase.apply(null, arguments);
        }
    }
    
    /* hook into completeRevealPhase to stop glitching when a round is complete */
    var original_completeRevealPhase = completeRevealPhase;
    completeRevealPhase = function() {
        if(!monika.present()) { return original_completeRevealPhase.apply(null, arguments); }
        
        try {
            var targetedSlot = monika.active_effects.round_targeted_glitching;
            if(targetedSlot) {
                monika.setGlitchingMarker(targetedSlot, monika.GLITCH_VISUAL, null);
                monika.stopCharacterGlitching(targetedSlot);
            }
            
            var editedPlayer = monika.active_effects.round_edit_glitching;
            if(editedPlayer) {
                monika.setGlitchingMarker(editedPlayer, monika.GLITCH_EDIT, null);
                monika.removeEditedDialogueStyle(editedPlayer);
            }
            
            var dialogueGlitchPlayer = monika.active_effects.round_dialogue_glitching;
            if(dialogueGlitchPlayer) {
                monika.setGlitchingMarker(dialogueGlitchPlayer, monika.GLITCH_REPEAT, null);
            }
            
            monika.active_effects.round_targeted_glitching = null;
            monika.active_effects.round_dialogue_glitching = null;
            monika.active_effects.round_edit_glitching = null;
        } catch (e) {
            console.error("[Monika] Error in pre-completeRevealPhase prep: "+e.toString());
        } finally {
            return original_completeRevealPhase.apply(null, arguments); 
        }
    }

    /* Hook into updateGameVisual to handle:
     * - pose transitions while Monika is masturbating
     * - fixing dialogue box glitches for characters that are changing dialogue
     * - deletion glitch effects (character image manipulation, dialogue fuckery)
     */
    var original_updateGameVisual = updateGameVisual;
    updateGameVisual = function(player) {
        if(!monika.present()) { return original_updateGameVisual.apply(null, arguments); }
        
        try {
            /* This is kind of a quick and dirty hack-- but we do these checks
             * here because this function is called pretty much in all phases of the game,
             * which means we don't have to hook into an entirely new function.
             */
            var monika_slot = monika.find_slot();
            if(players[monika_slot].stage === 9 && monika.active_effects.glitch_masturbation == null) {
                monika.start_masturbating();
            }
            
            if(players[monika_slot].stage === 10 && monika.active_effects.glitch_masturbation != null) {
                monika.finish_masturbating();
            }
            
            monika.active_effects.corrupted_dialogue[player-1] = null;
            if(monika.active_effects.overflow_text[player-1]) {
                monika.resetDialogueBoxStyles(player);
            }
            
            monika.setGlitchingMarker(player, monika.GLITCH_REPEAT, null);
        } catch (e) {
            console.error("[Monika] Error in pre-updateGameVisual prep: "+e.toString());
        } finally {
            original_updateGameVisual(player);
        }
        
        if(monika.active_effects.round_delete_glitching) {
            var glitch_data = monika.active_effects.round_delete_glitching;
            var glitch_player = players[glitch_data.slot];
            
            var re = new RegExp(glitch_data.original_label, 'gi');
            var re2 = new RegExp(glitch_player.label, 'gi');
            
            var original_dialogue = $gameDialogues[player-1].html();
            
            var modified_dialogue = original_dialogue.replace(re, function (match) {
                return monika.generate_glitch_text(match.length + getRandomNumber(0, 5))
            });
            
            modified_dialogue = modified_dialogue.replace(re2, function (match) {
                return monika.generate_glitch_text(match.length)
            });
            
            if(glitch_data.slot === player) {
                monika.set_image_from_canvas($gameImages[player-1], glitch_data.cv);
                
                // lightly pepper a deleted character's dialogue with glitch chars
                modified_dialogue = monika.random_character_replacement(monika.inline_glitch_chars, modified_dialogue, 0.1)
            }
            
            $gameDialogues[player-1].html(modified_dialogue);
        }
        
        // dialogue-modifying effects will be undone at next visuals update for this player
        if(monika.active_effects.round_dialogue_glitching === player) {
            if(monika.force_zalgo_corruption || Math.random() < 0.5) {
                monika.zalgoCorruptDialogue(player);
            } else {
                monika.repeatCorruptDialogue(player); 
            }
        } 
        
        if(monika.active_effects.round_edit_glitching === player) {
            monika.applyEditedDialogueStyle(player);
        }
        
        if(players[player] && players[player].folder === 'opponents/monika/' && players[player].stage === 9) {
            var current_img = $gameImages[player-1].attr('src').substr(17);
            monika.when_loaded($gameImages[player-1], function () {
                console.log("[Monika] image loaded, glitching to new pose...");
                monika.glitch_pose_transition(player, current_img, 0, 500);
            });
            
            /*
            if(!$gameImages[player-1][0].complete) {
                // Wait for image to load before glitching
                $gameImages[player-1].one("load", function () {
                    console.log("[Monika] image loaded, glitching to new pose...");
                    monika.glitch_pose_transition(player, current_img, 0, 500);
                });
            } else {
                // Image already loaded, glitch now
                console.log("[Monika] Immediately glitching to new pose...");
                monika.glitch_pose_transition(player, current_img, 0, 500);
            }
            */
        }
    }
}
