import json
import os
import urllib.request
import urllib.parse
import sys

def download_from_api(api_json_path, out_dir, allowed_prefixes, exact_matches, exclude_prefixes):
    os.makedirs(out_dir, exist_ok=True)
    
    with open(api_json_path, 'r', encoding='utf-16') as f:
        data = json.loads(f.read())
        
    pages = data['query']['pages']
    all_images = []
    
    for page_id, page_info in pages.items():
        if 'images' in page_info:
            for img in page_info['images']:
                title = img['title']
                
                filename = title.replace('File:', '')
                if not filename.endswith('.png'):
                    continue
                    
                is_excluded = any(filename.startswith(p) for p in exclude_prefixes)
                if is_excluded:
                    continue
                    
                is_allowed = any(filename.startswith(p) for p in allowed_prefixes)
                is_exact = filename in exact_matches
                
                if is_allowed or is_exact:
                    all_images.append(title)
                    
    print(f"Found {len(all_images)} images to download in {api_json_path}")
    
    batch_size = 50
    for i in range(0, len(all_images), batch_size):
        batch = all_images[i:i+batch_size]
        titles = '|'.join([urllib.parse.quote(f) for f in batch])
        api_url = f"https://slaythespire.wiki.gg/api.php?action=query&titles={titles}&prop=imageinfo&iiprop=url&format=json"
        
        req = urllib.request.Request(api_url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36'})
        
        try:
            response = urllib.request.urlopen(req).read().decode('utf-8')
            img_data = json.loads(response)
            img_pages = img_data['query']['pages']
            
            for p_id, p_info in img_pages.items():
                if 'imageinfo' in p_info:
                    url = p_info['imageinfo'][0]['url']
                    filename = urllib.parse.unquote(url.split('/')[-1]).split('?')[0]
                    # Clean filename by removing StS2 prefix and spaces
                    clean_name = filename.replace('StS2_', '').replace('StS2 ', '')
                    out_path = os.path.join(out_dir, clean_name)
                    
                    if not os.path.exists(out_path):
                        print(f"Downloading {clean_name}...")
                        img_req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36'})
                        try:
                            img_bytes = urllib.request.urlopen(img_req).read()
                            with open(out_path, 'wb') as img_f:
                                img_f.write(img_bytes)
                        except Exception as e:
                            print(f"  Failed: {e}")
        except Exception as e:
            print(f"Error fetching batch: {e}")

if __name__ == '__main__':
    download_from_api(
        r'C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\relics_api.json',
        r'C:\Users\Gorim\Pictures\STS Mod Images\STS Relics',
        allowed_prefixes=['StS2'],
        exact_matches=['Necronomicon.png'],
        exclude_prefixes=['StS2 CardIcon']
    )
    download_from_api(
        r'C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\potions_api.json',
        r'C:\Users\Gorim\Pictures\STS Mod Images\STS Potions',
        allowed_prefixes=['StS2'],
        exact_matches=['FruitJuice.png'],
        exclude_prefixes=['StS2 CardIcon']
    )
