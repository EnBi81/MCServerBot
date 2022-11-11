import pip

try:
    import requests
except ModuleNotFoundError as err:
    pip.main(['install', 'requests'])

try:
    import bs4
except ModuleNotFoundError as err:
    pip.main(['install', 'beautifulsoup4'])

try:
    import re
except ModuleNotFoundError as err:
    pip.main(['install', 'regex'])

# import nonexistentpackagepipwilldefinitelynotfindpleaseohgoddontdownloadit
# for testing purposes
