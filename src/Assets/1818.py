f = open('Tex.txt', 'r+')
s = []
for i in range(197):
    s.append(f.readline())
    x = str(i)
    while len(x) < 3:
        x = '0' + x
    x = ' ' + x
    s[i] = s[i][0] + x + s[i][1:]
    print(s[i], end = '')
