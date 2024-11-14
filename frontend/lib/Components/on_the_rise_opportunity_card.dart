import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';

class OnTheRiseOpportunityCard extends StatelessWidget {
  final Opportunity opportunity;

  OnTheRiseOpportunityCard({
    required this.opportunity,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        // Check if the screen is narrow (e.g., mobile) or wide (e.g., tablet/desktop)
        bool isSmallScreen = constraints.maxWidth < 600;

        return Container(
          color: Color(0xFFCCFFE5),
          child: isSmallScreen
              ? Column(
                  children:
                      _buildColumnContent(), // Stack image and details vertically
                )
              : Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children:
                      _buildRowContent(), // Place image and details side-by-side
                ),
        );
      },
    );
  }

  List<Widget> _buildColumnContent() {
    return [
      // Image section
      Stack(
        children: [
          Image.network(
            'https://picsum.photos/200',
            fit: BoxFit.cover,
            height: 100,
            width: double.infinity,
          ),
          Positioned(
            top: 0,
            left: 0,
            child: Container(
              padding: const EdgeInsets.all(4.0),
              color: Colors.black54,
              child: Text(
                opportunity.category.toString().split('.').last,
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                ),
              ),
            ),
          ),
        ],
      ),

      SizedBox(width: 16, height: 16), // Spacing between image and text

      // Description section

      Padding(
        padding: EdgeInsets.all(8.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisSize: MainAxisSize.max,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    opportunity.name,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                ),
                SizedBox(width: 8),
                TextButton(onPressed: null, child: Text("Detalhes")),
              ],
            ),
            SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text("${opportunity.price.toString()}€/Pessoa"),
                SizedBox(width: 8),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      opportunity.location.toString().split('.').last,
                    ),
                    SizedBox(width: 8),
                    IconButton.filled(onPressed: null, icon: Icon(Icons.star)),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    ];
  }

  List<Widget> _buildRowContent() {
    return [
      // Image section
      Expanded(
        flex: 1,
        child: Stack(
          children: [
            Image.network(
              'https://picsum.photos/200',
              fit: BoxFit.cover,
              height: 100,
              width: double.infinity,
            ),
            Positioned(
              top: 0,
              left: 0,
              child: Container(
                padding: const EdgeInsets.all(4.0),
                color: Colors.black54,
                child: Text(
                  opportunity.category.toString().split('.').last,
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 16,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),

      SizedBox(width: 16, height: 16), // Spacing between image and text

      // Description section
      Expanded(
        flex: 1,
        child: Padding(
          padding: EdgeInsets.all(8.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.max,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(
                      opportunity.name,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style:
                          TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                    ),
                  ),
                  SizedBox(width: 8),
                  TextButton(onPressed: null, child: Text("Detalhes")),
                ],
              ),
              SizedBox(height: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text("${opportunity.price.toString()}€/Pessoa"),
                  SizedBox(width: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        opportunity.location.toString().split('.').last,
                      ),
                      SizedBox(width: 8),
                      IconButton.filled(
                          onPressed: null, icon: Icon(Icons.star)),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    ];
  }
}
