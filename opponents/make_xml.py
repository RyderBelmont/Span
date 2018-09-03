# -*- coding: utf-8 -*-

import sys
import imp
if sys.version_info[0] == 2:
	imp.reload(sys)
	sys.setdefaultencoding('utf8')
import xml.etree.ElementTree as ET
import xml.dom.minidom as minidom
import datetime

#tags that relate to ending sequences
ending_tag = "ending" #name for the ending
ending_gender_tag = "ending_gender" #player gender the ending is shown to
ending_preview_tag = "gallery_image" # image to use for the preview in the gallery
screen_tag = "screen"
text_tag = "text"
x_tag = "x"
y_tag = "y"
width_tag = "width"
arrow_tag = "arrow"
ending_tags = [ending_tag, ending_gender_tag, ending_preview_tag, screen_tag, text_tag, x_tag, y_tag, width_tag, arrow_tag]

#sets of possible targets for lines
one_word_targets = ["target", "filter", "silent"]
multi_word_targets = ["targetStage", "alsoPlaying", "alsoPlayingStage", "alsoPlayingHand", "oppHand", "hasHand", "totalMales", "totalFemales", "targetTimeInStage", "alsoPlayingTimeInStage", "timeInStage", "consecutiveLosses", "totalAlive", "totalExposed", "totalNaked", "totalMasturbating", "totalFinished", "totalRounds", "saidMarker", "notSaidMarker", "alsoPlayingSaidMarker", "alsoPlayingNotSaidMarker", "targetSaidMarker", "targetNotSaidMarker", "priority"] #these will need to be re-capitalised when writing the xml
lower_multi_targets = [t.lower() for t in multi_word_targets]
all_targets = one_word_targets + lower_multi_targets

#default images and text for most cases
def get_cases_dictionary():
	d = {}#male pre-strip scenes
	d["male_human_must_strip"] = [{"key":"male_human_must_strip", "image":"interested", "text":"What are you going to take off, ~name~?"}]
	d["male_must_strip"] = [{"key":"male_must_strip", "image":"interested", "text":"What are you going to take off, ~name~?"}]
	d["male_removing_accessory"] = [{"key":"male_removing_accessory", "image":"sad", "text":"You're only taking off your ~clothing~, ~name~? That doesn't seem fair."}]
	d["male_removing_minor"] = [{"key":"male_removing_minor", "image":"calm", "text":"I guess your ~clothing~ is something, at least."}]
	d["male_removing_major"] = [{"key":"male_removing_major", "image":"interested", "text":"Finally getting ~name~ out of his ~clothing~!"}]
	d["male_chest_will_be_visible"] = [{"key":"male_chest_will_be_visible", "image":"interested", "text":"I guess it's time to see that chest of yours, ~name~!"}]
	d["male_crotch_will_be_visible"] = [{"key":"male_crotch_will_be_visible", "image":"horny", "text":"I guess you have to show 'that' to me now, ~name~..."}]
	
	#male stripping
	d["male_removed_accessory"] = [{"key":"male_removed_accessory", "image":"calm", "text":"At least you have less small stuff to take off now."}]
	d["male_removed_minor"] = [{"key":"male_removed_minor", "image":"happy", "text":"Maybe we can get you out of some large stuff now, ~name~."}]
	d["male_removed_major"] = [{"key":"male_removed_major", "image":"interested", "text":"You look better without your ~clothing~, ~name~."}]
	d["male_chest_is_visible"] = [{"key":"male_chest_is_visible", "image":"interested", "text":"Nice chest, ~name~."}]
	d["male_small_crotch_is_visible"] = [{"key":"male_small_crotch_is_visible", "image":"calm", "text":"That's... smaller than I was expecting... not that anything is wrong with that, ~name~."}]
	d["male_medium_crotch_is_visible"] = [{"key":"male_medium_crotch_is_visible", "image":"awkward", "text":"Well then... shall we continue the game?"}]
	d["male_large_crotch_is_visible"] = [{"key":"male_large_crotch_is_visible", "image":"shocked", "text":"That is massive! How do you even manage with that thing, ~name~?"}]
	
	#male masturbating
	d["male_must_masturbate"] = [{"key":"male_must_masturbate", "image":"interested", "text":"Time to show your skills, ~name~..."}]
	d["male_start_masturbating"] = [{"key":"male_start_masturbating", "image":"horny", "text":"You're going to have to go until you're done, ~name~..."}]
	d["male_masturbating"] = [{"key":"male_masturbating", "image":"horny", "text":"Keep going, ~name~..."}]
	d["male_finished_masturbating"] = [{"key":"male_finished_masturbating", "image":"shocked", "text":"Wow... uh... I guess you're done then..."}]
	
	#female pre-strip
	d["female_human_must_strip"] = [{"key":"female_human_must_strip", "image":"interested", "text":"What are you going to take off, ~name~?"}]
	d["female_must_strip"] = [{"key":"female_must_strip", "image":"interested", "text":"What are you going to take off, ~name~?"}]
	d["female_removing_accessory"] = [{"key":"female_removing_accessory", "image":"sad", "text":"You're only taking off your ~clothing~, ~name~? That doesn't seem fair."}]
	d["female_removing_minor"] = [{"key":"female_removing_minor", "image":"calm", "text":"I guess your ~clothing~ is something, at least."}]
	d["female_removing_major"] = [{"key":"female_removing_major", "image":"interested", "text":"Finally getting ~name~ out of her ~clothing~!"}]
	d["female_chest_will_be_visible"] = [{"key":"female_chest_will_be_visible", "image":"interested", "text":"I guess it's time to see those tits of yours, ~name~!"}]
	d["female_crotch_will_be_visible"] = [{"key":"female_crotch_will_be_visible", "image":"horny", "text":"I guess you have to show 'that' to me now, ~name~..."}]
	
	#female stripping
	d["female_removed_accessory"] = [{"key":"female_removed_accessory", "image":"calm", "text":"At least you have less small stuff to take off now."}]
	d["female_removed_minor"] = [{"key":"female_removed_minor", "image":"happy", "text":"Maybe we can get you out of some large stuff now, ~name~."}]
	d["female_removed_major"] = [{"key":"female_removed_major", "image":"interested", "text":"You look better without your ~clothing~, ~name~."}]
	d["female_small_chest_is_visible"] = [{"key":"female_small_chest_is_visible", "image":"interested", "text":"Those are nice, ~name~."}]
	d["female_medium_chest_is_visible"] = [{"key":"female_medium_chest_is_visible", "image":"horny", "text":"Nice tits, ~name~."}]
	d["female_large_chest_is_visible"] = [{"key":"female_large_chest_is_visible", "image":"shocked", "text":"How do you even manage with those things, ~name~. Is your back okay?"}]
	d["female_crotch_is_visible"] = [{"key":"female_crotch_is_visible", "image":"shocked", "text":"It's so pretty, ~name~..."}]
	
	#female masturbating
	d["female_must_masturbate"] = [{"key":"female_must_masturbate", "image":"interested", "text":"Time to show your skills, ~name~..."}]
	d["female_start_masturbating"] = [{"key":"female_start_masturbating", "image":"horny", "text":"You're going to have to go until you're done, ~name~..."}]
	d["female_masturbating"] = [{"key":"female_masturbating", "image":"horny", "text":"Keep going, ~name~..."}]
	d["female_finished_masturbating"] = [{"key":"female_finished_masturbating", "image":"shocked", "text":"Wow... uh... I guess you're done then..."}]
	
	#victory
	d["game_over_victory"] = [{"key":"game_over_victory", "image":"happy", "text":"I WON!"}]
	
	return d

#get the cases for when the character is still in the game (all clothed stages, and nude)
def get_playing_cases_dictionary():
	d = {}
	#quality of hand
	d["swap_cards"] = [{"key":"swap_cards", "image":"calm", "text":"I'll exchange ~cards~ cards."}]
	d["good_hand"] = [{"key":"good_hand", "image":"happy", "text":"I've got a good hand."}]
	d["okay_hand"] = [{"key":"okay_hand", "image":"calm", "text":"I've got an okay hand."}]
	d["bad_hand"] = [{"key":"bad_hand", "image":"sad", "text":"I've got a bad hand."}]
	
	return d

#cases where the player can strip (all stages until nude)
def get_stripping_cases_dictionary():
	d = {}
	
	#stripping
	d["stripped"] = [{"key":"stripped", "image":"sad", "text":"I miss my ~clothing~ already..."}]
	d["must_strip_winning"] = [{"key":"must_strip_winning", "image":"loss", "text":"Well, I guess it had to be my turn eventually..."}]
	d["must_strip_normal"] = [{"key":"must_strip_normal", "image":"loss", "text":"I guess I lost, huh?"}]
	d["must_strip_losing"] = [{"key":"must_strip_losing", "image":"loss", "text":"I lost again? But... I have less clothes than everyone else!"}]
	d["stripping"] = [{"key":"stripping", "image":"strip", "text":"I guess I'll just take off my ~clothing~..."}]
	return d
	
#default images and text for being nude
def get_nude_cases_dictionary():
	d = {}
	d["stripped"] = [{"key":"stripped", "image":"sad", "text":"I miss my ~clothing~ already..."}] #there's still a stripped case when they're nude
	d["must_masturbate"] = [{"key":"must_masturbate", "image":"loss", "text":"I guess I lost..."}]
	d["must_masturbate_first"] = [{"key":"must_masturbate_first", "image":"loss", "text":"Y-You want me to do what?!"}]
	d["start_masturbating"] = [{"key":"start_masturbating", "image":"starting", "text":"I guess I have to do 'that' now, huh?"}]
	
	return d

#default images and text for masturbating
def get_masturbating_cases_dictionary():
	d = {}
	d["masturbating"] = [{"key":"masturbating", "image":"calm", "text":"How long do I have to keep going for?"}]
	d["heavy_masturbating"] = [{"key":"heavy_masturbating", "image":"heavy", "text":"Mmmmmmmm...."}]
	d["finishing_masturbating"] = [{"key":"finishing_masturbating", "image":"finishing", "text":"I'm cumming!"}]
	return d

#default images and text for being finished
def get_finished_Cases_dictionary():
	d = {}
	d["finished_masturbating"] = [{"key":"finished_masturbating", "image":"finished", "text":"I'm done..."}]
	d["game_over_defeat"] = [{"key":"game_over_defeat", "image":"calm", "text":"Congrats, ~name~... I can't believe I lost..."}]
	return d

#default images for being selected at the start of the game and the game starting.
# These have no default text since they're new and we don't want to force everyone to use them
def get_start_cases_dictionary():
	d = {}
	d["selected"] = []
	d["game_start"] = []
	return d


#get a set of cases from the dictionaries. First try stage-specific from the character's data, then general entries from the character's data, then stage-specific from the default data, then general cases from the default data.
def get_cases(player_dictionary, default_dictionary, key, stage):
	image_formats = ["png", "jpg", "jpeg", "gif", "gifv"] #image file format extensions
	out_list = []
	full_key = "%d-%s" % (stage, key)
	
	result_list = list()

        def is_generic_line(line_data):
	        for target_type in all_targets:
			if target_type in line_data:
				return False
                return True
	
	def have_generic_line(lines):
		for line_data in lines:
                        if is_generic_line(line_data):
                                return True
                return False
	
	using_player = False
	have_generic_entry = False
	
	#check character's data
	if full_key in player_dictionary:
		result_list += player_dictionary[full_key]
		
		#check if whe have a line that doesn't have any targets or filters
		#because we need at least one line that doesn't have one
		if have_generic_line(result_list):
			have_generic_entry = True
			using_player = True
		
	if key in player_dictionary:
                for line_data in player_dictionary[key]:
                        # Don't add completely generic lines to a given stage when a
                        # stage-specific generic case exist for that stage,
                        # but do add targeted lines (because it's too complicated to
                        # look for matching targeted cases and it shouldn't cause any
                        # conflicts with workarounds for incorrectly added defaults).
                        if not is_generic_line(line_data) or not have_generic_entry:
		                result_list.append(line_data)

		if have_generic_line(result_list):
			have_generic_entry = True
			using_player = True
	
	backup_list = None
	
	#use the default data if there are no player-specific lines available
	if key in default_dictionary:
		backup_list = default_dictionary[key]
		if not have_generic_entry:
			result_list += backup_list
	
	#debug
	#if not using_player:
		#print "not using player line for key %s, stage %d" % (key, stage)
	
	#convert image formats
	#print "result list", result_list #for debug purposes
	for i, line_data in enumerate(result_list):
		line_data = dict(line_data) #use a copy of the line_data entry
		#because if we copy it then changing the stage number for images (below) for lines that don't have stage numbers
		#will use the first stage number that doesn't have a stage-specific version for all the stages where the generic line is used
	
		image = line_data["image"]
		text = line_data["text"]
		if len(image) <= 0:
			#if the character entry doesn't include an image, use default image
			image = backup_list[i % len(backup_list)]["image"] #use i'th image in default dictionary, if possible. wrap around if backup list isn't long enough
		
		#if the image name doesn't include a stage, prepend the current stage
		if not image[0].isdigit():
			image = "%d-%s" % (stage, image)
		
		#if no file extension, assume .png
		if "." not in image:
			image += ".png"
		else:
			name, extention = image.rsplit(".", 1)
			if extention not in image_formats:
				#if the image name doesn't end with a known image format, assume it's a .png file that just happens to have a . in its name
				image += ".png"
		
		line_data["image"] = image
		
		#out_list.append( (image+".png", text) ) don't use this
		out_list.append( line_data ) #because we switched to using dictionaries
	
	return out_list

#add a single emenent (initially used so I can add a tag named "tag")
#now it also handles targets, which are optional
#now it takes a series of lines for a particular stage, and adds all the <case> and <state> elements for the given list of lines
def create_case_xml(base_element, lines):
	#one_word_targets = ["target", "filter"]
	#targets = one_word_targets + ["targetstage"]
	
	#step 1: sort the lines by case (situation, and any targets)
	#this means that once the case changes, we know that the script won't see that case again
	#give them a key to define an order
	for line_data in lines:
		sort_key = line_data["key"]
		if "conditions" in line_data:
			for condition in line_data["conditions"]:
				sort_key += "," + "count-" + condition[0]
		for target_type in all_targets:
			if target_type in line_data:
				sort_key += "," + target_type + ":" +line_data[target_type]
		line_data["sort_key"] = sort_key
		
	#now do the sorting
	lines.sort(key=lambda l: l["sort_key"])
	
	#step 2: iterate through the list of lines
	current_sort = "" #which case combination we're currently looking at. initially nothing
	case_xml_element = None #current XML element, add states to this
	
	for line_data in lines:
		if line_data["sort_key"] != current_sort:
			#this is a new key
			current_sort = line_data["sort_key"]
			
			#make a new <case> element in the xml
			tag_list = {"tag":line_data["key"]} #every case needs a "tag" value that denotes the situation
			
			for target_type in one_word_targets:
				if target_type in line_data:
					tag_list[target_type] = line_data[target_type]
			
			#need to re-capitalise multi-word target names
			for ind, lower_case_target in enumerate(lower_multi_targets):
				if lower_case_target in line_data:
					capital_word = multi_word_targets[ind]
					tag_list[capital_word] = line_data[lower_case_target]
	
			case_xml_element = ET.SubElement(base_element, "case", tag_list) #create the <case> element in the xml

			if "conditions" in line_data:
				for condition in line_data["conditions"]:
					ET.SubElement(case_xml_element, "condition", {"filter": condition[0], "count": condition[1]})

		#now add the individual line
		#remember that this happens regardless of if the <case> is new
		attrib = {"img": line_data["image"]}
		if "marker" in line_data:
			attrib["marker"] = line_data["marker"]
		ET.SubElement(case_xml_element, "state", attrib).text = line_data["text"] #add the image and text

#add several values to the XML tree
#specifically, adds the <case> and <state> elements to a <stage> base_element
def add_values(base_element, player_dictionary, default_dictionary, stage):
	if type(default_dictionary) != list:
		default_dictionary = [default_dictionary]
	for d in default_dictionary:
		for key in list(d.keys()):
			contents = get_cases(player_dictionary, d, key, stage)
			#add the target values, if any
			target_tags = []
			case = create_case_xml(base_element, contents) #add the case element to the XML tree
			#for img, text in contents: #no longer used
			#	ET.SubElement(case, "state", img=img).text = text #add the states to the case

#manually prettify xml code (because the standard method doesn't seem to work on windows)
def manual_prettify_xml(elem, level=0, isLast=False):
	indent = "    "
	if elem.text is None and len(elem) > 0:
		elem.text = "\n" + (level + 1) * indent
	if isLast:
		elem.tail = "\n" + (level - 1) * indent
	else:
		elem.tail = "\n" + (level) * indent
		
	if elem.tag in ["stage", "wardrobe", "timer", "start", "behaviour", "epilogue", "screen", "text", "tags"]:
		elem.tail = "\n" + elem.tail
		
	if elem.tag == "opponent":
		elem.text = "\n" + elem.text
	
	for ind, subelem in enumerate(elem):
		is_last = ind == len(elem) - 1
		manual_prettify_xml(subelem, level + 1, is_last)
	return elem
			
#write the xml file to the specified filename
def write_xml(data, filename):
	main_dict = get_cases_dictionary()
	plyr_dict = get_playing_cases_dictionary()
	strp_dict = get_stripping_cases_dictionary()
	nude_dict = get_nude_cases_dictionary()
	mstb_dict = get_masturbating_cases_dictionary()
	fnsh_dict = get_finished_Cases_dictionary()
	strt_dict = get_start_cases_dictionary()
	

	#f = open(filename)
	o = ET.Element("opponent")
	mydate = datetime.datetime.now()
	o.insert(0, ET.Comment("This file was machine generated by make_xml.py version 1.54 in " + mydate.strftime("%B") + " " + mydate.strftime("%Y") +". Please do not edit it directly without preserving your improvements elsewhere or your changes may be lost the next time this file is generated."))
	ET.SubElement(o, "first").text = data["first"]
	ET.SubElement(o, "last").text = data["last"]

	#label
	for stage in data["label"]:
		if stage == 0:
			ET.SubElement(o, "label").text = data["label"][stage]
		else:
			ET.SubElement(o, "label", stage=stage).text = data["label"][stage]

	ET.SubElement(o, "gender").text = data["gender"]
	ET.SubElement(o, "size").text = data["size"]
	ET.SubElement(o, "timer").text = data["timer"]

	#intelligence
	for stage in data["intelligence"]:
		if stage == 0:
			ET.SubElement(o, "intelligence").text = data["intelligence"][stage]
		else:
			ET.SubElement(o, "intelligence", stage=stage).text = data["intelligence"][stage]

	#tags
	tags_elem = ET.SubElement(o, "tags")
	character_tags = set(data["character_tags"])
	for tag in character_tags:
		ET.SubElement(tags_elem, "tag").text = tag
		
	#start image
	start = ET.SubElement(o, "start")
	start_data = data["start"] if "start" in data else ["0-calm,So we'll be playing strip poker... I hope we have fun."]
	start_count = len(start_data)
	for i in range(0, start_count):
		start_image, start_text = start_data[i].split(",", 1)
		ET.SubElement(start, "state", img=start_image+".png").text = start_text
	
	#wardrobe
	clth = ET.SubElement(o, "wardrobe")
	clothes = data["clothes"]
	clothes_count = len(clothes)
	for i in range(clothes_count - 1, -1, -1):
		pname, lname, tp, pos, num = (clothes[i] + ",").split(",")[:5]
                clothesattr = {"proper-name":pname, "lowercase":lname, "type":tp, "position":pos}
                if num=="plural":
                        clothesattr["plural"] = "true"
		ET.SubElement(clth, "clothing", clothesattr)
	
	#behaviour
	bh = ET.SubElement(o, "behaviour")
	for stage in range(0, clothes_count):
		s = ET.SubElement(bh, "stage", id=str(stage))
                if stage == 0:
		        add_values(s, data, [strt_dict], stage)
		add_values(s, data, [main_dict, plyr_dict, strp_dict], stage)
		if stage == 0:
			for el in s.findall("./case[@tag='stripped']"):
				s.remove(el)

	#nude stage
	stage += 1
	s = ET.SubElement(bh, "stage", id=str(stage))
	add_values(s, data, [main_dict, plyr_dict, nude_dict], stage)
	
	#masturbating stage
	stage += 1
	s = ET.SubElement(bh, "stage", id=str(stage))
	add_values(s, data, [main_dict, mstb_dict], stage)
	for el in s.findall("./case[@tag='game_over_victory']"):
		s.remove(el)
			
	#finished stage
	stage += 1
	s = ET.SubElement(bh, "stage", id=str(stage))
	add_values(s, data, [main_dict, fnsh_dict], stage)
	for el in s.findall("./case[@tag='game_over_victory']"):
		s.remove(el)
	
	#endings
	if "endings" in data:
		#for each ending
		for ending in data["endings"]:
			ending_xml = ET.SubElement(o, "epilogue", gender=ending["gender"])
			
			if 'img' in ending:
				ending_xml.set('img', ending['img'])
			
			ET.SubElement(ending_xml, "title").text = ending["title"]
			
			#for each screen in an ending
			for screen in ending["screens"]:
				screen_xml = ET.SubElement(ending_xml, "screen", img=screen["image"])
				
				#for each text box on a screen
				for text_box in screen["text_boxes"]:
					text_box_xml = ET.SubElement(screen_xml, "text")
					ET.SubElement(text_box_xml, x_tag).text = text_box[x_tag]
					ET.SubElement(text_box_xml, y_tag).text = text_box[y_tag]
					#width and arrow are optional
					if width_tag in text_box:
						ET.SubElement(text_box_xml, width_tag).text = text_box[width_tag]
					if arrow_tag in text_box:
						ET.SubElement(text_box_xml, arrow_tag).text = text_box[arrow_tag]
					ET.SubElement(text_box_xml, "content").text = text_box[text_tag]
	
	#done
	
	
	
	#this outputs compact/non-pretty xml
	#tree = ET.ElementTree(o)
	#tree.write(filename, xml_declaration=True)
	
	#this is supposed to prettify
	#xml_prettystr = minidom.parseString(ET.tostring(o)).toprettyxml(indent="    ")
	#with open(filename, "w") as f:
	#	f.write(pretty_xml)
	
	#manual prettify
	pretty_xml = manual_prettify_xml(o)
	ET.ElementTree(pretty_xml).write(filename, encoding='UTF-8', xml_declaration=True)

#add an ending to the 
def add_ending(ending, d):
	ending = dict(ending)

	if len(list(ending.keys())) <= 0:
		#this is an empty ending, so don't add anything
		return
	
	#check for required values
	if "title" not in ending:
		print("Error - ending \"%s\" does not have a title." % (str(ending)))
		return
		
	if "gender" not in ending:
		print("Error - ending \"%s\" does not have a gender specified." % (str(ending)))
		return
		
	if "screens" not in ending:
		print("Error - ending \"%s\" does not have any screens." % (str(ending)))
		return
	
	#either get the endings data from the dictionary, or make a new endings variable and add that to the dictionary
	endings = None
	if "endings" in d:
		endings = d["endings"]
	else:
		endings = list()
		d["endings"] = endings
		
	endings.append(ending)
	
#handle the ending data
def handle_ending_string(key, content, ending, d):
	if key == ending_tag:
		#this is a new ending, so store the previous ending (if any)
		add_ending(ending, d)
		#reset the ending data
		ending.clear()
		#and add the title of the new ending
		ending["title"] = content
		return
	elif key == ending_gender_tag:
		if len(content) <= 0: #if the gender wasn't specified, use "any"
			content = "any"
		ending["gender"] = content
		return
	elif key == ending_preview_tag:
		if len(content) > 0:
			ending['img'] = content
		return
		
	#get the screens variable
	screens = None
	if "screens" in ending:
		screens = ending["screens"]
	else:
		#or make one, if it doesn't already exist
		screens = list()
		ending["screens"] = screens
		
	#get the current screen
	screen = None
	if len(screens) >= 1:
		screen = screens[-1]
	
	#background image for a screen - makes a new screen
	if key == screen_tag:
		screen = dict()
		screens.append(screen)
		screen["image"] = content
		screen["text_boxes"] = list()
		return
	
	#make sure we have a screen ready, because the other tags are specific to a screen
	if screen is None:
		print("Error - using tag \"%s\" with value \"%s\", without a screen varaible - use the \"%s\" tag first to put this information on that screen." % (key, content, screen_tag))
		return
	
	text_boxes = screen["text_boxes"]
	
	#the actual text of the text box. this makes a new text box
	if key == text_tag:
		text_box = dict()
		text_box[text_tag] = content
		text_boxes.append(text_box)
		return
		
	#get the current text box for the current screen
	text_box = None
	if len(text_boxes) >= 1:
		text_box = text_boxes[-1]
	else:
		print("Error - trying to use tag \"%s\" with value \"%s\", without making a text box. Use the \"%s\" tag first." % (key, content, text_tag))
		return
	
	#x position. Can be a css value, or "centered"
	if key == x_tag:
		text_box[x_tag] = content
		return
	
	#y position. Is a css value.
	elif key == y_tag:
		text_box[y_tag] = content
		return
	
	#width of a text box. defaults to 20%
	elif key == width_tag:
		text_box[width_tag] = content
		return
		
	#direction of the dialogue box arrow (if anything)
	elif key == arrow_tag:
		text_box[arrow_tag] = content
		return
		
	
#read in a character's data
def read_player_file(filename):
	main_dict = get_cases_dictionary()
	plyr_dict = get_playing_cases_dictionary()
	strp_dict = get_stripping_cases_dictionary()
	nude_dict = get_nude_cases_dictionary()
	mstb_dict = get_masturbating_cases_dictionary()
	fnsh_dict = get_finished_Cases_dictionary()
	strt_dict = get_start_cases_dictionary()
	
	case_names = list(main_dict.keys()) + list(plyr_dict.keys()) + list(strp_dict.keys()) + list(nude_dict.keys()) + list(mstb_dict.keys()) + list(fnsh_dict.keys()) + list(strt_dict.keys())
	
	d = {}
	
	ending = dict()
	
	stage = -1
	
	f = open(filename, 'r')
	for line_number, line in enumerate(f):
		line = line.strip()
		
		line_data = dict() #all of the lines data:
		#key is the stage and situation in which the line should be used. includes a stage number for stage-specific lines
		#image = the image filename (if no extension, assumed to be png)
		#target = if the line targets a particular other character
		#targetStage = if the line targets a particular stage for a particular character
		#filter = if the line targets a particular tag
		
		if len(line) <= 0 or line[0]=='#': #use # as a comment character, and skip empty lines
			continue
		
		#check for characters that can't be used
		if sys.version_info[0] == 2:
			skip_line = False
			try:
				# In utf-8, characters using umlauts are actually encoded as two separate characters
				# so we need to try to decode the entire line instead of individual characters
				line.decode('utf-8')
			except UnicodeDecodeError:
				# Find out which character
				problem_character = ""
				for c in line:
					try:
						c.decode('utf-8')
					except UnicodeDecodeError:
						problem_character = c
						break

				if (len(problem_character) > 0):
					print("Unable to decode character %s in line %d: \"%s\"" % (problem_character, line_number, line))
				else:
					print("Unable to decode line \"%s\" in line %d: " % (line, line_number))

				skip_line = True
				break

			if skip_line:
				continue
		
		#split the lines, then check for malformed entries
		try:
			key, text = line.split("=", 1)
		except ValueError:
			#this helps to find lines that are misformed 
			print("Unable to split line %d: \"%s\"" % (line_number, line))
			continue
		
		key = key.strip().lower()
		
		stripped = text.strip()
		
		
		#now deal with any possible targets and filters
		target_type = "skip" #reset any previous target type. this should only be used if there's a target present, but setting it here just in case
		if ',' in key:
			target_parts = key.split(',')
			key = target_parts[0]
			targets = target_parts[1:]
			for t in targets:
			
				try:
					target_type, target_value = t.split(":")
				except ValueError:
					#make sure the target has a format we can understand
					print("Invalid targeting for line %d - \"%s\". Skipping line." % (line_number, line))
					target_type = "skip"
					stripped = ""
					target_value = "N/A"
				
				target_type = target_type.strip()
				target_value = target_value.strip()
				
				#make sure there's a target. Can I check the data here to make sure that a target is valid?
				if len(target_value) <= 0:
					print("No target value specified for line %d - \"%s\". Skipping line." % (line_number, line))
					target_type = skip
					stripped = ""
				
				#now actually process valid targets
				#valid targets
				if target_type in all_targets:
					line_data[target_type] = target_value
					
				elif target_type == "skip":
					#skip this target type
					pass

				elif target_type == "marker":
					line_data["marker"] = target_value
					pass

				elif target_type.startswith("count-"):
					condition_filter = target_type[6::]
					if "conditions" not in line_data:
						line_data["conditions"] = [[condition_filter, target_value]]
					else: line_data["conditions"].append([condition_filter, target_value])
					
				else:
					#unknown target type
					print("Error - unknown target type \"%s\" for line %d - \"%s\". Skipping line." % (target_type, line_number, line))
					stripped = "" #make the script skip this line
					
				if target_type == "targetstage":
					#print a warning if they used a targetStage without a target
					have_target = False
					for other_target_data in targets:
						if "target:" in other_target_data:
							have_target = True
							break
					if not have_target:
						print("Warning - using a targetStage for line %d - \"%s\" without using a target value" % (line_number, line))
		
		
		#if the key contains a -, it belongs to a specific stage
		if '-' in key:
			stg, part_key = key.rsplit('-', 1)
			
			#if it starts with a * use the current stage
			if stg[0] == '*':
				key = "%d-%s" % (stage, part_key)
			
			#negative numbers count from the end. -1 is finished, -2 is masturbating, -3 is nude. -4 is the last layer of clothing, and so on.
			#using negative numbers assumes that they are after all the clothes entries
			elif stg[0] == '-' and stg[1:].isdigit():
				key = "%d-%s" % (stage + 4 + int(stg), part_key)
		else:
			part_key = key
		
		#cases, these can be repeated
		if part_key in case_names:
		
			line_data["key"] = part_key
		
			if stripped == "" or stripped == ",":
				#if there's no entry, skip it.
				continue
				
			if ',' not in text:
				#img, desc = "", text
				line_data["image"] = ""
				line_data["text"] = text
			else:
				img,desc = text.split(",", 1) #split into (image, text) pairs
				line_data["image"] = img
				line_data["text"] = desc
				
			#print "adding line", line	
			
			if key in d:
				d[key].append(line_data) #add it to existing list of responses
			else:
				d[key] = [line_data] #make a new list of responses
				
		#clothes is a list
		elif key == "clothes":
			stage += 1
			if "clothes" in d:
				d["clothes"].append(stripped)
			else:
				d["clothes"] = [stripped]

        #intelligence is written as
        #   intelligence=bad
        #   intelligence=good,3
        #this means to start at bad intelligence and switch to good starting at stage 3
        #   The label can be changed in the same manner
		elif key in ("intelligence", "label"):
                        parts = stripped.split(",", 1)
			(from_stage, value) = (0 if len(parts) == 1 else parts[1], parts[0])
			if key in d:
				d[key][from_stage] = value
			else:
				d[key] = {from_stage: value}

		#tags for the character i.e. blonde, athletic, cute
		#tags can be written as either:
		#	tag=blonde
		#	tag=athletic
		#or as
		#	tags=blond, athletic
		elif key == "tag":
			if "character_tags" in d:
				d["character_tags"].append(stripped)
			else:
				d["character_tags"] = [stripped]

		elif key == "tags":
			character_tags = [tag.strip() for tag in stripped.split(',')]
			if "character_tags" in d:
				d["character_tags"] = d["character_tags"] + character_tags
			else:
				d["character_tags"] = character_tags

		elif key == "marker":
			if "markers" in d:
				d["markers"].append(stripped)
			else:
				d["markers"] = [stripped]

		#write start lines last to first
		elif key == "start":
			if key in d:
				d[key].append(text)
			else:
				d[key] = [text]

		#this tag relates to an ending squence
		#use a different function, because it's quite complicated
		elif key in ending_tags:
			handle_ending_string(key, stripped, ending, d)
		
		#other values are single lines. These need to be in the data, even if the value is empty
		else:
			d[key] = text
	
	#add the final ending (if it exists)
	add_ending(ending, d)
	
    #set default intelligence, if the writer doesn't set it
	if "intelligence" not in d:
		d["intelligence"] = [["0", "average"]]

	return d

#make the meta.xml file
def make_meta_xml(data, filename):
	o = ET.Element("opponent")
	
	enabled = "true" if "enabled" not in data or data["enabled"] == "true" else "false"
	ET.SubElement(o, "enabled").text = enabled
	
	values = ["first","last","label","pic","gender","height","from","writer","artist","description","has_ending","layers","character_tags","release"]
	
	for value in values:
		content = ""
		if value in data:
			content = data[value]
		if value == "pic":
			if content == "":
				content = "0-calm"
			content += ".png"
		
		if value == "layers":
			#the number of layers of clothing is taken directly from the clothing data
			content = str(len(data["clothes"]))

                if value == "label":
                        content = data["label"][0]
			
		if value == "has_ending":
			#say whether or not they have an ending based on whether they have any ending data or not
			content = "true" if "endings" in data else "false"

                if value == "character_tags":
                        tags_elem = ET.SubElement(o, "tags")
                        character_tags = set(data["character_tags"])
	                for tag in character_tags:
		                ET.SubElement(tags_elem, "tag").text = tag
		else:
		        ET.SubElement(o, value).text = content
		
	#ET.ElementTree(o).write(filename, xml_declaration=True)
	
	pretty_xml = manual_prettify_xml(o)
	ET.ElementTree(pretty_xml).write(filename, encoding="UTF-8", xml_declaration=True)

#make the marker.xml file
def make_markers_xml(data, filename):
	if "markers" in data:
		o = ET.Element("markers")
		markers = data["markers"]
		for marker_data in markers:
			name, scope, desc = marker_data.split(",", 2)
			if scope == "public":
				scope = "Public"
			elif scope == "private":
				scope = "Private"
			ET.SubElement(o, "marker", **{"name":name, "scope":scope}).text = desc
		
		pretty_xml = manual_prettify_xml(o)
		ET.ElementTree(pretty_xml).write(filename, encoding="UTF-8", xml_declaration=True)

#read the input data, the write the xml files
def make_xml(player_filename, out_filename, meta_filename=None, marker_filename=None):
	player_dictionary = read_player_file(player_filename)
	write_xml(player_dictionary, out_filename)
	if meta_filename is not None:
		make_meta_xml(player_dictionary, meta_filename)
	if marker_filename is not None:
		make_markers_xml(player_dictionary, marker_filename)

#make the xml files using the given arguments
#python make_xml <character data file> <behaviour.xml output file> <meta.xml output file>
if __name__ == "__main__":
	if len(sys.argv) <= 1:
		print("Please give the name of the dialogue file to process into XML files")
		exit()
	behaviour_name = "behaviour.xml"
	meta_name = "meta.xml"
	marker_name = "markers.xml"
	if len(sys.argv) > 2:
		behaviour_name = sys.argv[2]
	if len(sys.argv) > 3:
		meta_name = sys.argv[3]
	if len(sys.argv) > 4:
		marker_name = sys.argv[4]
		
	make_xml(sys.argv[1], behaviour_name, meta_name, marker_name)


#make_xml.py converts angled brackets and ampersands into their html symbol equivalents.
#This is probably a clumsy way of converting some of them back for italics and symbols for behaviour.xml, but it works.
#Also converted here are the hand quality words, which make_xml converts to lower case
replacements = {'&lt;i&gt;':'<i>', '&lt;/i&gt;':'</i>', '&lt;I&gt;':'<i>', '&lt;/I&gt;':'</i>', '&amp;':'&', '="high card"':'="High Card"', '="one pair"':'="One Pair"', '="two pair"':'="Two Pair"', '="three of a kind"':'="Three of a Kind"', 'hand="straight"':'hand="Straight"', '="flush"':'="Flush"', '="full house"':'="Full House"', '="four of a kind"':'="Four of a Kind"', '="straight flush"':'="Straight Flush"', '="royal flush"':'="Royal Flush"', '>~silent~':' silent="">', 'png"> ':'png">', '…':'...', '“':'"', '”':'"'} #By only converting angled brackets when they're part of italics, characters like Nugi-chan can still use them as displayed characters without creating invalid xmls.

lines = []
with open(behaviour_name) as infile:
    for line in infile:
        for src, target in replacements.iteritems():
            line = line.replace(src, target)
        lines.append(line)
with open(behaviour_name, 'w') as outfile:
    for line in lines:
        outfile.write(line)
