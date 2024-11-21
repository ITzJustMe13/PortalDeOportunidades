import 'package:flutter/material.dart';
import 'package:frontend/Components/dynamic_details_button.dart';
import 'package:frontend/Models/Opportunity.dart';

class OnTheRiseOpportunityCard extends StatelessWidget {
  final Opportunity opportunity;

  const OnTheRiseOpportunityCard({
    super.key,
    required this.opportunity,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        bool isSmallScreen = constraints.maxWidth <450;

        return Container(
          height: isSmallScreen ? 300 : 150,
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
    return [_buildImageSection(), _buildDetailsSection()];
  }

  List<Widget> _buildRowContent() {
    return [
      Expanded(
        flex: 1,
        child: _buildImageSection(),
      ),
      Expanded(flex: 1, child: _buildDetailsSection()),
    ];
  }

  Widget _buildImageSection() {
    return Stack(
      children: [
        Image.network(
          'https://picsum.photos/200',
          fit: BoxFit.cover,
          height: 150,
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
    );
  }

  Widget _buildDetailsSection() {
    return Padding(
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
              DynamicDetailsButton()
            ],
          ),
          SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text("${opportunity.price.toString()}€ /Pessoa"),
              SizedBox(width: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    opportunity.location.name,
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(width: 8),
                  IconButton.filled(onPressed: null, icon: Icon(Icons.star)),
                ],
              ),
            ],
          ),
        ],
      ),
    );
  }
}
