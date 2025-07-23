#!/usr/bin/env bash
set -e

if [ $# -eq 1 ]; then
  SUFFIX=$1
  SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
  "$SCRIPT_DIR/copy_template.sh" "TestLibrary/$SUFFIX" "$SUFFIX" "TestLibrary/Template"
  "$SCRIPT_DIR/copy_template.sh" "UnitTests/$SUFFIX" "$SUFFIX" "UnitTests/Template"
  exit 0
fi

if [ $# -lt 2 ]; then
  echo "Usage: $0 <target_dir> <suffix> [template_dir]"
  echo "   or: $0 <suffix>   # to run both default copy commands"
  exit 1
fi

TARGET_DIR=$1
SUFFIX=$2
TEMPLATE_DIR=${3:-Template}

mkdir -p "$TARGET_DIR"

find "$TEMPLATE_DIR" -type f | while read -r file
do
  relative_path="${file#$TEMPLATE_DIR/}"
  mkdir -p "$TARGET_DIR/$(dirname "$relative_path")"

  filename="$(basename "$file")"
  extension="${filename##*.}"
  base="${filename%.*}"

  # Insert suffix before .cs extension if file is .cs, else append as before
  if [[ "$extension" == "cs" ]]; then
    new_filename="${base}${SUFFIX}.cs"
  else
    new_filename="${filename}${SUFFIX}"
  fi

  target_path="$TARGET_DIR/$(dirname "$relative_path")/$new_filename"

  # Adjust namespace and using lines for .cs files
  if [[ "$extension" == "cs" ]]; then
    awk -v sfx="$SUFFIX" '
      {
        gsub(/\.Template\./, "." sfx ".");
        if ($0 ~ /^namespace / || $0 ~ /^using /) {
          sub(/Template/, sfx);
        }
        print
      }
    ' "$file" > "$target_path"
  else
    cp "$file" "$target_path"
  fi
done