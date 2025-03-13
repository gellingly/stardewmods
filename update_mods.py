import glob
from pathlib import Path
import zipfile
import os
import shutil
from dotenv import load_dotenv

load_dotenv()

MODS_FOLDER = Path(os.environ["MODS_FOLDER"])
DOWNLOADS_FOLDER = Path(os.environ["DOWNLOADS_FOLDER"])


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
        print(f"Extracted all files to {extract_to}")


def main():
    for file in glob.glob(f"{DOWNLOADS_FOLDER}/*.zip"):
        print(Path(file).stem)
        unzip_to = DOWNLOADS_FOLDER / Path(file).stem
        unzip_file(file, unzip_to)

        def clean_up():
            print(f"failed to update {Path(file).stem}")

        # Get subfolder = name of mod
        entries = os.listdir(unzip_to)

        # Iterate through entries to find the first subfolder
        if len(entries) > 1:
            clean_up()
            continue

        mod_name = entries[0]
        mod = MODS_FOLDER / mod_name
        if not os.path.isdir(mod):
            print(f"mod {mod} is not a directory")
            clean_up()
            continue

        if not os.path.isfile(mod / "manifest.json"):
            print(f"mod {mod} has no manifest.json")
            clean_up()
            continue

        config = mod / "config.json"
        if os.path.isfile(config):
            shutil.move(config, unzip_to / mod_name / "config.json")

        shutil.rmtree(mod)
        shutil.move(unzip_to / mod_name, mod)

        shutil.rmtree(unzip_to)
        os.remove(file)
        print(f"finished {Path(file).stem}")


if __name__ == "__main__":
    main()
