/********************************************************************************
 This file contains the variables and functions that form the gallery screens of
 the game.
 ********************************************************************************/

function GEnding(player, ending){
	this.player = player;
	this.gender = $(ending).attr('gender');
	this.image = player.folder + $(ending).find('screen').eq(0).attr('img');
	this.title = $(ending).find('title').html();
	this.unlocked = save.hasEnding(player.id, this.title);
	//Same as in spniEpilogue.js
	this.screens = [];
	var $end = this;
	$(ending).find('screen').each(function(){
		var image = player.folder + $(this).attr("img").trim();
		var textBoxes = [];
		$(this).find('text').each(function(){
			var x = $(this).find("x").html().trim();
			var y = $(this).find("y").html().trim();
			var w = $(this).find("width").html();
			var a = $(this).find("arrow").html();

			if (w) {
				w = w.trim();
			}
			if (!w || (w.length <= 0)) {
				w = "20%"; //default to text boxes having a width of 20%
			}
			if (a) {
				a = a.trim().toLowerCase();
				if (a.length >= 1) {
					a = "arrow-" + a; //class name for the different arrows. Only use if the writer specified something.
				}
			} else {
				a = "";
			}
			if (x.toLowerCase() == "centered") {
				x = getCenteredPosition(w);
			}
			var text = $(this).find("content").html().trim();
			textBoxes.push({x:x, y:y, width:w, arrow:a, text:text});
		});
		$end.screens.push({image:image, textBoxes:textBoxes});
	});
}

 /**********************************************************************
  *****                   Gallery Screen UI Elements               *****
  **********************************************************************/

$genderTypeButtons = [$('#gallery-gender-all'), $('#gallery-gender-any'), $('#gallery-gender-male'), $('#gallery-gender-female')];
$galleryEndings = $('#gallery-endings-block').children();
$galleryPrevButton = $('#gallery-prev-page-button');
$galleryNextButton = $('#gallery-next-page-button');
$galleryStartButton = $('#gallery-start-ending-button');
$selectedEndingPreview = $('#selected-ending-previev');
$selectedEndingLabels = [$('#selected-ending-title'), $('#selected-ending-character'), $('#selected-ending-gender')];

function loadGalleryScreen(){
	screenTransition($titleScreen, $galleryScreen);
	loadGalleryEndings();
}

function backGalleryScreen(){
	screenTransition($galleryScreen, $titleScreen);
}

/* opponent listing file */
var galleryEndings = [];
var allEndings = [];
var anyEndings = [];
var maleEndings = [];
var femaleEndings = [];
var isEndingLoaded = [];
var galleryPage = 0;
var galleryPages = -1;
var epp = 20;
var selectedEnding = -1;
var GALLERY_GENDER = 'all';
var loadIndex = 0;

function loadGalleryEndings(){
	if(allEndings.length>0){
		return;
	}
	for(var i=0; i<loadedOpponents.length; i++){
		isEndingLoaded.push(false);
	}
	for(var i=0; i<loadedOpponents.length; i++){
		if(loadedOpponents[i].ending){
			loadEndingXml(i);
		}
		else{
			isEndingLoaded[i] = true;
		}
	}

	//I'm not using setInterval on purpose, although it shouldn't be necessary
	setTimeout(fetchLoadedEndings,1);

	//I don't know why but sometimes start button enables itself at start.
	//Strangely it only happens when I refresh browser (firefox) after anabling it by picking valid epilouge
	$galleryStartButton.attr('disabled', true);
}

function fetchLoadedEndings(){
	if(loadIndex >= isEndingLoaded.length){
		return;
	}
	//If it's false go straight to setting new timeout
	if(isEndingLoaded[loadIndex]!=false){
		while(loadIndex<isEndingLoaded.length && isEndingLoaded[loadIndex]!=false){
			if(isEndingLoaded[loadIndex]==true){
				loadIndex++;
			}
			else{
				var endings = isEndingLoaded[loadIndex];
				endings.each(function(){
					var gending = new GEnding(loadedOpponents[loadIndex], this)
					allEndings.push( gending );
					switch(gending.gender){
						case 'male': maleEndings.push(gending);
						break;
						case 'female': femaleEndings.push(gending);
						break;
						default: anyEndings.push(gending);
						break;
					}
				});

				loadIndex++;
				galleryGender(GALLERY_GENDER);
			}
		}
	}
	setTimeout(fetchLoadedEndings, 200);
}

function loadEndingXml(index){
	$.ajax({
		type: "GET",
		url: loadedOpponents[index].folder + 'behaviour.xml',
		dataType: "text",
		success: function(xml){
			var endings = $(xml).find('epilogue');
			isEndingLoaded[index] = endings;
		}
	});
}

function loadEndingThunbnail(element, ending){
	element.removeClass('empty-thumbnail');
	if(ending.unlocked){
		element.removeClass('unlocked-thumbnail');
		element.css('background-image','url(\''+ending.image+'\')');
	}
	else{
		element.css('background-image', '');
		element.addClass('unlocked-thumbnail');
	}
}

function loadThumbnails(){
	var i=0;
	for(; i<epp && epp*galleryPage+i<galleryEndings.length; i++){
		loadEndingThunbnail($galleryEndings.eq(i), galleryEndings[epp*galleryPage+i]);
	}
	for( ; i<epp; i++){
		$galleryEndings.eq(i).removeClass('unlocked-thumbnail');
		$galleryEndings.eq(i).addClass('empty-thumbnail');
		$galleryEndings.eq(i).css('background-image', 'none');
	}
}

function galleryGender(gender){
	GALLERY_GENDER = gender;
	$('.gallery-gender-button').css('opacity', 0.4);
	switch(gender){
		case 'male':
			galleryEndings = maleEndings;
			$('#gallery-gender-male').css('opacity', 1);
			break;
		case 'female':
			galleryEndings = femaleEndings;
			$('#gallery-gender-female').css('opacity', 1);
			break;
		case 'any':
			galleryEndings = anyEndings;
			$('#gallery-gender-any').css('opacity', 1);
			break;
		default:
			galleryEndings = allEndings;
			$('#gallery-gender-all').css('opacity', 1);
			break;
	}
	galleryPages = Math.ceil(galleryEndings.length/parseFloat(epp));
	galleryPage = 0;
	$galleryPrevButton.attr('disabled', true);
	if(galleryPages==1){
		$galleryNextButton.attr('disabled', true);
	}
	else{
		$galleryNextButton.attr('disabled', false);
	}
	loadThumbnails();
}

function galleryNextPage(){
	galleryPage++;
	loadThumbnails();
	$galleryEndings.css('opacity', '');
	$galleryPrevButton.attr('disabled', false);
	if(galleryPage+1==galleryPages){
		$galleryNextButton.attr('disabled', true);
	}
}

function galleryPrevPage(){
	galleryPage--;
	loadThumbnails();
	$galleryEndings.css('opacity', '');
	$galleryNextButton.attr('disabled', false);
	if(galleryPage==0){
		$galleryPrevButton.attr('disabled', true);
	}
}

function selectEnding(i){
	selectedEnding = epp*galleryPage+i;
	var ending = galleryEndings[selectedEnding];
	if(ending.unlocked){
		$galleryStartButton.attr('disabled', false);
		chosenEpilogue = ending;
		$selectedEndingLabels[0].html(ending.title);
	}
	else{
		$galleryStartButton.attr('disabled', true);
		chosenEpilogue = -1;
		$selectedEndingLabels[0].html('');
	}
	$galleryEndings.css('opacity', '');
	$galleryEndings.eq(i).css('opacity', 1);
	loadEndingThunbnail($selectedEndingPreview, ending);
	$selectedEndingLabels[1].html(ending.player.label);
	$selectedEndingLabels[2].html(ending.gender);
	switch(ending.gender){
		case 'male':
			$selectedEndingLabels[2].removeClass('female-style');
			$selectedEndingLabels[2].addClass('male-style');
			break;
		case 'female':
			$selectedEndingLabels[2].removeClass('male-style');
			$selectedEndingLabels[2].addClass('female-style');
			break;
		default:
			$selectedEndingLabels[2].removeClass('female-style');
			$selectedEndingLabels[2].removeClass('male-style');
	}
}

function doEpilogueFromGallery(){
	if($nameField.val()){
		players[HUMAN_PLAYER].label = $nameField.val();
	}
	else{
		switch(chosenEpilogue.gender){
			case "male": players[HUMAN_PLAYER].label = "Mister"; break;
			case "female" : players[HUMAN_PLAYER].label = "Missy"; break;
			default: players[HUMAN_PLAYER].label = (players[HUMAN_PLAYER].gender=="male")?"Mister":"Missy";
		}
	}
	clearEpilogueBoxes();
	epilogueScreen = 0; //reset epilogue position in case a previous epilogue played before this one
	epilogueText = 0;
	progressEpilogue(0); //initialise buttons and text boxes
	screenTransition($galleryScreen, $epilogueScreen); //currently transitioning from title screen, because this is for testing
	$epilogueSelectionModal.modal("hide");
}
