import 'package:flutter/material.dart';

class OpportunityImages extends StatefulWidget {
  @override
  _OpportunityImagesState createState() => _OpportunityImagesState();
}

class _OpportunityImagesState extends State<OpportunityImages> {
  final List<String> _imagePaths = [
    'assets/images/opp.jpg',
    'assets/images/opp.jpg',
    'assets/images/opp.jpg',
  ];
  final PageController _pageController = PageController();
  int _currentPage = 0;

  void _nextPage() {
    if (_currentPage < _imagePaths.length - 1) {
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
                itemCount: _imagePaths.length,
                onPageChanged: (int index) {
                  setState(() {
                    _currentPage = index;
                  });
                },
                itemBuilder: (context, index) {
                  return ClipRRect(
                    borderRadius: BorderRadius.circular(10.0),
                    child: Image.asset(
                      _imagePaths[index],
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
          children: List.generate(_imagePaths.length, (index) {
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
