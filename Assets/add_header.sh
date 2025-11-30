#!/bin/bash

HEADER="/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/
"

TARGET_DIR="Scripts"   # change if needed

find "$TARGET_DIR" -type f -name "*.cs" | while read -r FILE
do
    # Check if file already contains the header
    if ! grep -q "Cardwheel — Non-Commercial, No-Modification License" "$FILE"; then
        echo "Adding header to: $FILE"
        # Create a temp file with header + original content
        (echo "$HEADER" && cat "$FILE") > "$FILE.tmp"
        mv "$FILE.tmp" "$FILE"
    else
        echo "Skipping (already contains header): $FILE"
    fi
done

echo "Done."
