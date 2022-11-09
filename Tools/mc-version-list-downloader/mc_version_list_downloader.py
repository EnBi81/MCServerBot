import requests
from bs4 import BeautifulSoup
import re
import json

result = {"versions": []}


def is_release_build_table(table):
    h3 = table.findPreviousSibling('h3')
    h3_text = h3.find('span').text
    if re.match(r'.*((Beta)|(Alpha)|(test)).*', h3_text) is None:
        return True


def is_row_invalid(row):
    row_text = row.get_text(" ", strip=True)
    if not re.match(r'.*((Upcoming)|(Server only)).*', row_text) is None:
        return True
    is_rowspan_cell = len(row.select('td[rowspan]')) > 0
    return is_rowspan_cell


def get_update_title(table):
    """Get the title of the update from a table."""
    update_name = table.select('td[rowspan]')
    if not update_name:
        update_name = table.select('tr')[1].select('td')
    regex = re.match(r'(.*)\(Guide\)', update_name[0].get_text(strip=True))
    filtered_text = re.sub(r' I', ' I', regex.group(1)).strip()
    # Part I and Part II caused issues with weird unicode characters, also strips trailing whitespace
    return filtered_text


def append_to_result(name, version_str, full_release, dl_link):
    result["versions"].append({
        "name": name,
        "version": version_str,
        "fullRelease": full_release,
        "downloadLink": dl_link,
    })


url = 'https://minecraft.fandom.com/wiki/Java_Edition_version_history'
response = requests.get(url)
soup: BeautifulSoup = BeautifulSoup(response.text, 'html.parser')

all_tables = soup.select('h3 + table.wikitable',)

filtered_tables = [table for table in all_tables if is_release_build_table(table)]


for table in filtered_tables:
    update_title = get_update_title(table)

    rows = table.select('tr')
    rows.pop(0)
    filtered_rows = [row for row in rows if not is_row_invalid(row)]

    # single rows cause name to be in the first cell of the row
    if len(filtered_rows) == 1:
        cells = filtered_rows[0].select('td')
        version = cells[1].get_text(strip=True)
        full_release_date = cells[-1].get_text(strip=True)
        append_to_result(update_title, version, full_release_date, "")
        continue

    for row in filtered_rows:
        cells = row.select('td')
        version = cells[0].get_text(strip=True)
        full_release_date = cells[-1].get_text(strip=True)
        append_to_result(update_title, version, full_release_date, "")

json.dump(result, open('mc_version_list.json', 'w'), indent=4)
