SEABOW_BIN = ./bin/Release/net7.0
ARGS = # seabow arguments

cmp-l:
	dotnet publish -c Release -r linux-x64 --self-contained

cmp-w:
	dotnet publish -c Release -r win-x64 --self-contained

cmp-d:
	dotnet publish -c Release -r osx-x64 --self-contained


run-l:
	$(SEABOW_BIN)/linux-x64/publish/seabow $(ARGS)

run-w:
	wine $(SEABOW_BIN)/win-x64/seabow.exe $(ARGS)