ACLOCAL_AMFLAGS = -I m4

CSC_V = $(CSC_V_$(V))
CSC_V_ = $(CSC_V_$(AM_DEFAULT_VERBOSITY))
CSC_V_0 = @echo [CSC] $@;

.SECONDEXPANSION:
%.exe : $$($$*_EXE_SRC) $$($$*_EXE_RESOURCES)
	$(CSC_V)$(CSC) $(CSCFLAGS) $($*_EXE_CSCFLAGS) -target:exe \
		-out:$@ $($*_EXE_SRC) $($*_EXE_RESOURCES)

%.dll : $$($$*_DLL_SRC) $$($$*_DLL_RESOURCES)
	$(CSC_V)$(CSC) $(CSCFLAGS) $($*_DLL_CSCFLAGS) \
		-target:library \
		-out:$@ $($*_DLL_SRC) $($*_DLL_RESOURCES)

pkglib_DATA = PulseAudio.dll
PulseAudio_DLL_SRC = 	src/AssemblyInfo.cs \
			src/Channel.cs \
			src/Context.cs \
			src/GLib.cs \
			src/MainLoop.cs \
			src/Operation.cs \
			src/PropList.cs \
			src/Sample.cs \
			src/ServerInfo.cs \
			src/Sink.cs \
			src/SinkInput.cs \
			src/Util.cs \
			src/Volume.cs

PulseAudio_DLL_CSCFLAGS = $(GLIB_SHARP_20_LIBS)

check_DATA = Test.dll
Test_DLL_SRC = 	tests/TestChannel.cs \
		tests/TestContext.cs \
		tests/TestGLib.cs \
		tests/TestMainLoop.cs \
		tests/TestPropList.cs \
		tests/TestSample.cs \
		tests/TestSink.cs \
		tests/TestUtil.cs \
		tests/TestVolume.cs
Test_DLL_CSCFLAGS = -pkg:nunit -pkg:glib-sharp-2.0 -r:PulseAudio.dll
TESTS_ENVIRONMENT = nunit-console
TESTS = Test.dll

CLEANFILES=$(wildcard *.dll)
