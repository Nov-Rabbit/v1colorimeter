# v1colorimeter

Development OS Platform:
Windows 7, 64 bit

Development Language:
    Visual C# 2010

Test Station Runnning Mode:
    Default Mode: Running with Colorimeter Connected.
    Demo Mode: Running with no Colorimeter. Mainly for test data analysis or algorithm tuning.
    
DUT Runnning Mode:
    Manual Mode: Need manually update the display test patterns
    Auto Mode: The DUT patterns can be changed through the send command
    
Basic Architecture:
    Configuration -- Configure the colorimeter before the test. More specifically, including the size/luminance/color calibration from standard. Encrypted. 
    Test          -- Normal Test mode. 
    Audit         -- Self audit. If fails, need re-configure. (Audit frequency controlled)
    Analysis      -- Test data analysis. No need for colorimeter connected.
    
Output:
    Configuration -- Color correction matrix and size normalization variable. 
    Test          -- Mura, Dead Pixel Amount, Uniformity, color, defect location.
    Audit         -- Pass/Fail boolean and failure error code.
    Analysis      -- Multiple intermediate attributes.

