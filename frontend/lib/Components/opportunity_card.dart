import 'package:flutter/material.dart';
import 'package:frontend/Components/dynamic_details_button.dart';
import 'package:frontend/Models/Opportunity.dart';

class OpportunityCard extends StatelessWidget {
  final Opportunity opportunity;

  const OpportunityCard({
    super.key,
    required this.opportunity,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        return Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Expanded(
              flex: 2,
              child: _buildImageSection(),
            ),
            Expanded(
              child: _buildDetailsSection(),
            )
          ],
        );
      },
    );
  }

  Widget _buildImageSection() {
    return Stack(
      children: [
        Image.network(
          'https://picsum.photos/200',
          fit: BoxFit.cover,
          height: double.infinity,
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
                fontSize: 14,
              ),
            ),
          ),
        ),
        Positioned(
          bottom: 0,
          left: 0,
          child: Container(
              padding: const EdgeInsets.all(4.0),
              color: Colors.black54,
              child:
                  Column(mainAxisAlignment: MainAxisAlignment.start, children: [
                Text(
                  "${opportunity.price.toString()}€ /Pessoa",
                  style: TextStyle(fontSize: 12, color: Colors.white),
                ),
                Text(
                  opportunity.location.toString().split('.').last,
                  style: TextStyle(fontSize: 14, color: Colors.white),
                ),
              ])),
        ),
      ],
    );
  }

  Widget _buildDetailsSection() {
    return Container(
      color: Color(0xFFD9D9D9),
      padding: EdgeInsets.all(8.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            opportunity.name,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
          ),
          SizedBox(
            width: double.infinity,
            child: Wrap(
              spacing: 8,
              crossAxisAlignment: WrapCrossAlignment.center,
              alignment: WrapAlignment.spaceBetween,
              children: [
                Flexible(
                  fit: FlexFit.loose,
                  flex: 1,
                  child: IconButton.filled(
                    onPressed: null,
                    icon: Icon(Icons.star),
                    style: IconButton.styleFrom(
                      padding: EdgeInsets.all(2),
                    ),
                  ),
                ),
                SizedBox(height: 8),
                Flexible(
                  fit: FlexFit.loose,
                  flex: 1,
                  child: DynamicDetailsButton(),
                ),
              ],
            ),
          )
        ],
      ),
    );
  }
}
