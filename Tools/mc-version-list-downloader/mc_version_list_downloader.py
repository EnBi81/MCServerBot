import requests
from bs4 import BeautifulSoup
import re
import json


def is_release_build_table(table):
    """Check if table contains release builds."""
    h3 = table.findPreviousSibling('h3')
    h3_text = h3.find('span').text
    if re.match(r'.*((Beta)|(Alpha)|(test)).*', h3_text) is None:
        return True


def is_row_invalid(row):
    """Checks if a row is invalid (e.g. contains Upcoming update)."""
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
    filtered_text = re.sub(r' ', ' ', regex.group(1)).strip()
    # Part I and Part II caused issues with weird unicode space characters, also strips trailing whitespace
    return filtered_text


def get_download_row(table):
    """Get the row containing the download link."""
    rows = table.select('tr')
    for row in rows:
        if re.match(r'.*Download.*', row.get_text(" ", strip=True)):
            return row


def get_server_link(td):
    """Get the server download link from a td element, returns none if it doesn't find it."""
    links = td.select('a')
    a = None
    while a is None and len(links) > 0:
        link = links.pop(0)
        match = re.match(r'.*((Server)|(Reuploaded server)).*', link.get_text(" ", strip=True))
        # reuploaded server occures in 1.7.5
        if match:
            a = link

    return a['href']


def get_download_link_from_row(cells):
    """Get the server download link from a list of cells."""
    url = f'https://minecraft.fandom.com/{cells[0].select("a")[0]["href"]}'
    resp = requests.get(url)
    soup = BeautifulSoup(resp.text, 'html.parser')

    link = None
    try:
        table = soup.select('table.infobox-rows')[0]
        row = get_download_row(table)
        link = get_server_link(row.select('td')[0])
    except Exception as e:
        print(f"Error on: {url}")
        print(e)

    return link if link else "Unknown"


def main():
    url = 'https://minecraft.fandom.com/wiki/Java_Edition_version_history'

    print(f'Getting list of Minecraft versions from {url}')

    # Get the main page
    response = requests.get(url)
    soup: BeautifulSoup = BeautifulSoup(response.text, 'html.parser')

    # Get all tables that contain versions
    all_tables = soup.select('h3 + table.wikitable',)

    # Filter out tables that don't contain release builds
    filtered_tables = [table for table in all_tables if is_release_build_table(table)]

    result = {"versions": []}

    # Process each table
    for i, table in enumerate(filtered_tables):
        # Get the update title
        update_title = get_update_title(table)

        # Get and filter rows, invalid rows are thrown away
        rows = table.select('tr')
        rows.pop(0)
        filtered_rows = [row for row in rows if not is_row_invalid(row)]

        # Process each row
        for row in filtered_rows:
            # Get the cells of the row
            cells = row.select('td')
            if len(cells) == 4:  # check if row has the title of the update
                cells.pop(0)  # drop title of the update

            # Get the version name
            version = cells[0].get_text(strip=True)

            # Get the release date
            full_release_date = cells[-1].get_text(strip=True)

            # Get the download link
            download_link = get_download_link_from_row(cells)

            # Add the version to the result
            result["versions"].append({
                "name": update_title,
                "version": version,
                "fullRelease": full_release_date,
                "downloadLink": download_link,
            })

        # Print progress
        print(f"{i+1}th version done out of {len(filtered_tables)}")

    # Write the result to a json file
    json.dump(result, open('mc_version_list.json', 'w'), indent=4)

    # Alert the user that the script is finished
    print(f'Versions saved to "mc_version_list.json" \n'
          f'There are {len(result["versions"])} versions in total.')


if __name__ == '__main__':
    main()
