import argparse
import glob
from pathlib import Path
import re
import webbrowser
import zipfile
import os
import shutil
from dotenv import load_dotenv

load_dotenv()

MODS_FOLDER = Path(os.environ["MODS_FOLDER"])
DOWNLOADS_FOLDER = Path(os.environ["DOWNLOADS_FOLDER"])
SMAPI_LOGS = Path(os.environ["SMAPI_LOGS"])


def unzip_file(zip_path, extract_to=None):
    """
    Unzips a given .zip file to the specified directory.

    Parameters:
    zip_path (str): The path to the .zip file.
    extract_to (str): The directory to extract files to. If None, extracts to the current directory.
    """
    if not os.path.isfile(zip_path):
        raise FileNotFoundError(f"The file {zip_path} does not exist.")

    with zipfile.ZipFile(zip_path, "r") as zip_ref:
        if extract_to is None:
            extract_to = os.path.dirname(zip_path)

        zip_ref.extractall(extract_to)
        print(f"  Extracted all files to {extract_to}")


def update_mods():
    for file in glob.glob(f"{DOWNLOADS_FOLDER}/*.zip"):
        print(Path(file).stem)
        unzip_to = DOWNLOADS_FOLDER / Path(file).stem
        unzip_file(file, unzip_to)
        os.remove(file)

        def clean_up():
            print(f"  failed to update {Path(file).stem}")

        # Get subfolder = name of mod
        entries = os.listdir(unzip_to)

        # Iterate through entries to find the first subfolder
        if len(entries) > 1:
            clean_up()
            continue

        mod_name = entries[0]
        mod = MODS_FOLDER / mod_name
        if not os.path.isdir(mod):
            print(f"  mod is not a directory")
            clean_up()
            continue

        if not os.path.isfile(mod / "manifest.json"):
            print(f"  mod has no manifest.json")
            clean_up()
            continue

        config = mod / "config.json"
        if os.path.isfile(config):
            shutil.move(config, unzip_to / mod_name / "config.json")

        shutil.rmtree(mod)
        shutil.move(unzip_to / mod_name, mod)

        shutil.rmtree(unzip_to)
        print(f"  finished {Path(file).stem}")


def open_links():
    with open(SMAPI_LOGS / "SMAPI-latest.txt", "r") as f:
        lines = f.readlines()

    for line in lines:
        if (x := re.search(r": (https?://.*) \(you have .*\)", line)) is not None:
            link = x.group(1)
            link = f"{link}?tab=files"
            print(link)
            webbrowser.open(link)  # Open the link in the default web browser


def parse_args():
    parser = argparse.ArgumentParser(description="Unzip and update mods.")
    parser.add_argument(
        "--open",
        action="store_true",
        help="Open links for mods that need updating according to the most recent SMAPI log",
    )
    return parser.parse_args()


def main():
    args = parse_args()
    if args.open:
        open_links()
    else:
        update_mods()


if __name__ == "__main__":
    main()
