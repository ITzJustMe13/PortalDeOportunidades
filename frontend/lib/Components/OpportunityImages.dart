import 'dart:convert'; // Import for base64 decoding
import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';

class OpportunityImages extends StatefulWidget {
  final Opportunity opportunity;

  const OpportunityImages({super.key, required this.opportunity});

  @override
  _OpportunityImagesState createState() => _OpportunityImagesState();
}

class _OpportunityImagesState extends State<OpportunityImages> {
  late final List<String> _imageBase64Strings;
  final PageController _pageController = PageController();
  int _currentPage = 0;

  @override
  void initState() {
    super.initState();
    // Extract the base64 strings from the opportunity's images
    _imageBase64Strings = widget.opportunity.opportunityImgs
        .map((img) => img.imageBase64)
        .toList();
  }

  void _nextPage() {
    if (_currentPage < _imageBase64Strings.length - 1) {
      setState(() {
        _currentPage++;
        _pageController.nextPage(
          duration: Duration(milliseconds: 300),
          curve: Curves.easeInOut,
        );
      });
    }
  }

  void _previousPage() {
    if (_currentPage > 0) {
      setState(() {
        _currentPage--;
        _pageController.previousPage(
          duration: Duration(milliseconds: 300),
          curve: Curves.easeInOut,
        );
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Stack(
          children: [
            SizedBox(
              height: 400,
              child: PageView.builder(
                controller: _pageController,
                itemCount: _imageBase64Strings.length,
                onPageChanged: (int index) {
                  setState(() {
                    _currentPage = index;
                  });
                },
                itemBuilder: (context, index) {
                  final bytes = base64Decode(_imageBase64Strings[index]);
                  return ClipRRect(
                    borderRadius: BorderRadius.circular(10.0),
                    child: Image.memory(
                      bytes,
                      fit: BoxFit.cover,
                    ),
                  );
                },
              ),
            ),
            Positioned(
              left: 10,
              top: 175,
              child: IconButton(
                icon: Icon(Icons.arrow_back_ios, color: Colors.white),
                onPressed: _previousPage,
              ),
            ),
            Positioned(
              right: 10,
              top: 175,
              child: IconButton(
                icon: Icon(Icons.arrow_forward_ios, color: Colors.white),
                onPressed: _nextPage,
              ),
            ),
          ],
        ),
        SizedBox(height: 10), // Space between images and indicator
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(_imageBase64Strings.length, (index) {
            return AnimatedContainer(
              duration: Duration(milliseconds: 300),
              margin: EdgeInsets.symmetric(horizontal: 5),
              width: 8.0,
              height: 8.0,
              decoration: BoxDecoration(
                color: _currentPage == index ? Colors.green : Colors.grey,
                shape: BoxShape.circle,
              ),
            );
          }),
        ),
      ],
    );
  }
}
