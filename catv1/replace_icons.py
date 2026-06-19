import os
import re

icon_map = {
    'M15.41,16.58L10.83,12L15.41,7.41L14,6L8,12L14,18L15.41,16.58Z': '&#xe5cb;',
    'M8.59,16.58L13.17,12L8.59,7.41L10,6L16,12L10,18L8.59,16.58Z': '&#xe5cc;',
    'M20,11V13H8L13.5,18.5L12.08,19.92L4.16,12L12.08,4.08L13.5,5.5L8,11H20Z': '&#xe5c4;',
    'M21,19V20H3V19L5,17V11C5,7.9 7.03,5.17 10,4.29C10,4.19 10,4.1 10,4A2,2 0 0,1 12,2A2,2 0 0,1 14,4C14,4.1 14,4.19 14,4.29C16.97,5.17 19,7.9 19,11V17L21,19M14,21A2,2 0 0,1 12,23A2,2 0 0,1 10,21': '&#xe7f4;',
    'M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.68 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97Z': '&#xe8b8;',
    'M21,7L9,19L3.5,13.5L4.91,12.09L9,16.17L19.59,5.59L21,7Z': '&#xe5ca;',
    'M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z': '&#xe5cd;',
    'M16,6L18.29,8.29L13.41,13.17L9.41,9.17L2,16.59L3.41,18L9.41,12L13.41,16L19.71,9.71L22,12V6H16Z': '&#xe8e5;',
    'M12,3L1,9L12,15L21,10.09V17H23V9M5,13.18V17.18L12,21L19,17.18V13.18L12,17L5,13.18Z': '&#xe80c;',
    'M16,13C15.71,13 15.38,13 15.03,13.05C16.19,13.89 17,15 17,16.5V19H23V16.5C23,14.17 19.33,13 17,13M8,13C5.67,13 2,14.17 2,16.5V19H14V16.5C14,14.17 10.33,13 8,13M8,11A3,3 0 0,0 11,8A3,3 0 0,0 8,5A3,3 0 0,0 5,8A3,3 0 0,0 8,11M15,11A3,3 0 0,0 18,8A3,3 0 0,0 15,5A3,3 0 0,0 12,8A3,3 0 0,0 15,11Z': '&#xe7fb;',
    'M12,18A6,6 0 0,1 6,12A6,6 0 0,1 12,6A6,6 0 0,1 18,12A6,6 0 0,1 12,18M20,15.31L23.31,12L20,8.69V4H15.31L12,0.69L8.69,4H4V8.69L0.69,12L4,15.31V20H8.69L12,23.31L15.31,20H20V15.31Z': '&#xe40a;',
    'M16,17V14H9V10H16V7L21,12L16,17M14,2A2,2 0 0,1 16,4V6H14V4H5V20H14V18H16V20A2,2 0 0,1 14,22H5A2,2 0 0,1 3,20V4A2,2 0 0,1 5,2H14Z': '&#xe879;',
    'M20,8L12,13L4,8V6L12,11L20,6M20,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V6C22,4.89 21.1,4 20,4Z': '&#xe158;',
    'M12,17A2,2 0 0,0 14,15C14,13.89 13.1,13 12,13A2,2 0 0,0 10,15A2,2 0 0,0 12,17M18,8A2,2 0 0,1 20,10V20A2,2 0 0,1 18,22H6A2,2 0 0,1 4,20V10C4,8.89 4.9,8 6,8H7V6A5,5 0 0,1 12,1A5,5 0 0,1 17,6V8H18M12,3A3,3 0 0,0 9,6V8H15V6A3,3 0 0,0 12,3Z': '&#xe897;',
    'M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z': '&#xe8f4;',
    'M12,4A4,4 0 0,1 16,8A4,4 0 0,1 12,12A4,4 0 0,1 8,8A4,4 0 0,1 12,4M12,14C16.42,14 20,15.79 20,18V20H4V18C4,15.79 7.58,14 12,14Z': '&#xe7fd;',
    'M7,10H12V22H7V10M14,2H19V22H14V2M0,16H5V22H0V16Z': '&#xe24b;',
    'M19,4H18V2H16V4H8V2H6V4H5C3.89,4 3,4.89 3,6V20A2,2 0 0,0 5,22H19A2,2 0 0,0 21,20V6C21,4.89 20.1,4 19,4M19,20H5V10H19V20M19,8H5V6H19V8Z': '&#xe916;',
    'M6.76,4.84L5.34,3.42C3.23,5.07 1.80,7.46 1.55,10.2H3.55C3.80,8.00 5.06,6.10 6.76,4.84M20.45,10.2H22.45C22.19,7.46 20.77,5.07 18.65,3.42L17.24,4.84C18.94,6.10 20.19,8.00 20.45,10.2M18.96,11.45C18.96,7.75 16.94,4.76 13.5,3.96V3.2C13.5,2.24 12.74,1.46 11.78,1.46C10.82,1.46 10.06,2.24 10.06,3.2V3.96C6.62,4.76 4.6,7.75 4.6,11.45V17.45L2.6,19.45V20.45H20.96V19.45L18.96,17.45M11.78,23.45C12.89,23.45 13.78,22.55 13.78,21.45H9.78C9.78,22.55 10.68,23.45 11.78,23.45Z': '&#xe8b3;',
    'M12,7V3H2V21H22V7H12M6,19H4V17H6V19M6,15H4V13H6V15M6,11H4V9H6V11M6,7H4V5H6V7M10,19H8V17H10V19M10,15H8V13H10V15M10,11H8V9H10V11M10,7H8V5H10V7M20,19H12V17H14V15H12V13H14V11H12V9H20V19M18,11H16V13H18V11M18,15H16V17H18V15Z': '&#xe8ef;',
    'M22,3H2C0.91,3.04 0.04,3.91 0,5V19C0.04,20.09 0.91,20.96 2,21H22C23.09,20.96 23.96,20.09 24,19V5C23.96,3.91 23.09,3.04 22,3M22,19H2V5H22V19M14,17V15.75C14,14.09 10.66,13.25 9,13.25C7.34,13.25 4,14.09 4,15.75V17H14M9,7A2.5,2.5 0 0,1 11.5,9.5A2.5,2.5 0 0,1 9,12A2.5,2.5 0 0,1 6.5,9.5A2.5,2.5 0 0,1 9,7M19,17H16V15H19V17M19,13H16V11H19V13M19,9H16V7H19V9Z': '&#xe32b;'
}

import glob

def replacer(match):
    full_text = match.group(0)
    # Extract attributes
    data_match = re.search(r'Data="([^"]+)"', full_text)
    if not data_match:
        return full_text
    
    data_val = data_match.group(1)
    if data_val not in icon_map:
        return full_text
        
    icon_code = icon_map[data_val]
    
    # Extract other important properties
    fill_match = re.search(r'Fill="([^"]+)"', full_text)
    fill = fill_match.group(1) if fill_match else "White"
    
    # If Fill is bound, we need to handle that
    if "{Binding" in fill or "{StaticResource" in fill:
        text_color = fill
    else:
        text_color = fill
        
    width_match = re.search(r'WidthRequest="([^"]+)"', full_text)
    height_match = re.search(r'HeightRequest="([^"]+)"', full_text)
    
    size = "20"
    if width_match:
        try:
            size = str(int(float(width_match.group(1))))
        except:
            size = "20"
            
    # Extract positioning or visibilities
    extras = []
    
    grid_col = re.search(r'Grid\.Column="([^"]+)"', full_text)
    if grid_col: extras.append(f'Grid.Column="{grid_col.group(1)}"')
    
    grid_row = re.search(r'Grid\.Row="([^"]+)"', full_text)
    if grid_row: extras.append(f'Grid.Row="{grid_row.group(1)}"')
    
    grid_rowspan = re.search(r'Grid\.RowSpan="([^"]+)"', full_text)
    if grid_rowspan: extras.append(f'Grid.RowSpan="{grid_rowspan.group(1)}"')
    
    is_visible = re.search(r'IsVisible="([^"]+)"', full_text)
    if is_visible: extras.append(f'IsVisible="{is_visible.group(1)}"')
    
    margin = re.search(r'Margin="([^"]+)"', full_text)
    if margin: extras.append(f'Margin="{margin.group(1)}"')
    
    opacity = re.search(r'Opacity="([^"]+)"', full_text)
    if opacity: extras.append(f'Opacity="{opacity.group(1)}"')
    
    horz_opt = re.search(r'HorizontalOptions="([^"]+)"', full_text)
    horz = horz_opt.group(1) if horz_opt else "Center"
    
    vert_opt = re.search(r'VerticalOptions="([^"]+)"', full_text)
    vert = vert_opt.group(1) if vert_opt else "Center"
    
    extra_str = " ".join(extras)
    if extra_str:
        extra_str = " " + extra_str
        
    # Generate Label
    return f'<Label FontFamily="MaterialIcons" Text="{icon_code}" TextColor="{text_color}" FontSize="{size}" HorizontalOptions="{horz}" VerticalOptions="{vert}"{extra_str} />'

views_dir = 'c:/Users/harri/OneDrive/Documents/Desktop/CAT/catv1/Views'
for filepath in glob.glob(os.path.join(views_dir, '*.xaml')):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # regex to match <Path ... /> or <Path ... >...</Path>
    # Note: re.DOTALL to match across newlines
    new_content = re.sub(r'<Path\s+[^>]+?/>', replacer, content, flags=re.DOTALL)
    
    if new_content != content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"Updated {os.path.basename(filepath)}")
