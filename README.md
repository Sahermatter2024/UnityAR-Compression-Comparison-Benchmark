# UnityAR-Compression-Comparison-Benchmark
This project dives into compression methods (LZ4, LZMA, Zip...) to enhance AR experiences on resource-limited devices. Elevate your AR game with efficient compression.

# Project Title

## Overview

The rapid advancement of digital twin technology poses challenges in resource limitations on mobile devices and computational inefficiencies affecting user experiences. This project explores multiple lossless compression algorithms within Unity's integrated AssetBundle and Addressable methods to optimize bundle sizes and visualization time in AR applications. The study delves into the efficiency of compression methods and generates statistical models for estimating mobile phone resources in AR-based mobile applications.

## Table of Contents

- [Introduction](#introduction)
- [System Design](#system-design)
- [Testbed](#testbed)
- [Testing Benchmark](#testing-benchmark)
- [Analysis](#analysis)
- [Conclusion](#conclusion)
- [Tools](#tools)
- Results and Datasample](#results-and-datasample)

## Introduction

Industry 4.0 technologies, including Digital Twins, Artificial Intelligence, Virtual Reality, and Augmented Reality, are reshaping industrial and educational sectors. Digital Twin (DT) technology, connecting physical objects to virtual counterparts, has applications in various fields. Extended Realities (XR) technologies, like AR and VR, complement digital twins, but their implementation on mobile devices faces challenges.

Unity, a prominent game development framework, introduces AssetBundle and Addressable Asset system methods to address these challenges. This project aims to optimize CPU time and RAM usage by evaluating compression algorithms (LZMA, LZ4, Gzip, Brotli, etc.) in the context of Unity's methods.

## System Design

### Testbed

The study employed a testbed comprising an Android mobile phone, a Unity-installed computer, and a specialized mobile application. The primary testbed, a Windows-based computer, ran the Unity software and hosted an Apache web server (XAMPP). The benchmark application, developed using C# in Unity v2021.3.12f1 and Vuforia AR-Engine v10.3.2, targeted Android 8.0 with API level 26 on a Lenovo mobile phone.

### Testing Benchmark

The testing benchmark included:

- **Testbeds**: Android mobile phone, Unity-installed computer, and a dedicated mobile application.
- **Data-sets**: Bundles containing 3D models and video media for compression evaluation.
- **Compression Techniques**: Mono Behaviour C# scripts and Android Libraries for compression and decompression.

Mono Behaviour C# scripts and libraries with different compression algorithms and loading functions were installed in the benchmark app. The CPU time and maximum RAM usage during tests were recorded.

## Analysis

A comprehensive analysis used 12 data-sets, including three industrial virtual laboratory 3D models with varying polygon counts. The study investigated compression algorithm performance with diverse data-sets, modifying 3D model structures and incorporating MP4 media content. Table 2 provides detailed information about each asset, showcasing the diverse nature of the data-sets used in the study.

## Conclusion

This project contributes valuable insights into optimizing AR applications through compression algorithms within Unity's AssetBundle and Addressable methods. The findings enhance our understanding of performance implications and provide practical recommendations for developers working on AR-based mobile applications.

## Tools

- Unity v2021.3.12f1
- Vuforia AR-Engine v10.3.2
- Lenovo mobile phone running 245 Android 8.0 with API level 26
- Apache web server solution stack package v3.3.0 239 (XAMPP)
- Addressable Package” version 1.19.19.
- Asset-Bundle Browser Tool
  
## Results and Datasample

- Complete data results are saved under the `Results` folder.
- Sample 3D models are saved under the `Datasample` folder.

Include relevant citations to the scientific literature and sources mentioned in the text.

