from os import listdir
from os.path import isfile
from os.path import join as joinpath

myPath = "tetris-tests-2015"
myProg = "Tetris/bin/Debug/Tetris.exe"

myjson = filter(lambda x: x.endswith('.json'), listdir(myPath))

for i in myjson:
    if isfile(joinpath(myPath,i)):
        print(i)
		
		