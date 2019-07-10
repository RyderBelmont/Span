#!/bin/bash

mkdir -p .public/opponents
cp -r css fonts img js player index.html version-info.xml .public
sed "s/__CI_COMMIT_SHA/${CI_COMMIT_SHA}/g" prod-config.xml > .public/config.xml
cp opponents/listing.xml .public/opponents
cp opponents/general_collectibles.xml .public/opponents

# tar may be the easiest way to copy an arbitrary
# list of files, keeping the directory structure.
# Include *.js and *.css to accomodate Monika.
find `python opponents/list_opponents.py` -iname "*.png" -o -iname "*.gif" -o -iname "*.jpg" -o -iname "*.xml" -o -iname "*.js" -o -iname "*.css" | tar -cT - | tar -C .public -x

python3 opponents/fill_linecount_metadata.py .public/opponents
python opponents/gzip_dialogue.py .public/opponents/*/behaviour.xml
python opponents/analyze_image_space.py .public/opponents

# Uncomment this to copy alternate costume files for deployment.
#python opponents/copy_event_alts.py .public/ ./ easter