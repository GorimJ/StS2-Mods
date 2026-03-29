import re
import os
import json
import urllib.request
import urllib.parse

from urllib.error import HTTPError

def main():
    html_path = 'C:/Users/Gorim/.gemini/antigravity/Slay the Spire 2 Modding/sts_enchants.html'
    out_dir = r'C:\Users\Gorim\Pictures\STS Enchants'
    os.makedirs(out_dir, exist_ok=True)
    
    with open(html_path, 'r', encoding='utf-8') as f:
        html = f.read()
        
    # Find all file names: href="/wiki/File:Enchant_Adroit.png"
    files = re.findall(r'href="/wiki/File:(Enchant_[^"]+\.png)"', html)
    files = list(set(files))
    
    print(f"Found {len(files)} enchantment files.")
    
    # Query the MediaWiki API for image URLs
    titles = '|'.join(['File:' + urllib.parse.quote(f) for f in files])
    api_url = f"https://slaythespire.wiki.gg/api.php?action=query&titles={titles}&prop=imageinfo&iiprop=url&format=json"
    
    req = urllib.request.Request(api_url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/122.0.0.0 Safari/537.36'})
    try:
        response = urllib.request.urlopen(req).read().decode('utf-8')
        data = json.loads(response)
        pages = data['query']['pages']
        
        for page_id, page_info in pages.items():
            if 'imageinfo' in page_info:
                url = page_info['imageinfo'][0]['url']
                # url looks like https://slaythespire.wiki.gg/images/1/1a/Enchant_Adroit.png
                filename = urllib.parse.unquote(url.split('/')[-1]).split('?')[0]
                # Remove Enchant_ prefix for cleaner names
                clean_name = filename.replace('Enchant_', '').replace('.png', '') + '.png'
                out_path = os.path.join(out_dir, clean_name)
                
                print(f"Downloading {filename} from {url} to {clean_name}...")
                
                # Download
                img_req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36'})
                img_data = urllib.request.urlopen(img_req).read()
                
                with open(out_path, 'wb') as img_f:
                    img_f.write(img_data)
                    
        print("Done!")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == '__main__':
    main()
