# v1colorimeter

Development OS Platform:
Windows 7, 64 bit

Development Language:
    Visual C# 2010

Software Architecture:
    Configuration -- Configure the colorimeter before the test. More specifically, including the size/luminance/color calibration from standard. Encrypted. 
    Test          -- Normal Test mode. 
    Audit         -- Self audit. If fails, need re-configure. (Audit frequency controlled)
    
Output:
    Configuration -- Color correction matrix and size normalization variable. 
    Test          -- Mura, Dead Pixel Amount, Uniformity, color, defect location.
    Audit         -- Pass/Fail boolean and failure error code.
    