printf -v day '%02d' "$1"
printf -v day_url '%d' "$1"

mkdir -p "day_$day"
cp -r day_XX/* "day_$day/"
curl -b 'SESSION.txt' "https://adventofcode.com/2023/day/$day_url/input" -o "day_$day/input.txt"
