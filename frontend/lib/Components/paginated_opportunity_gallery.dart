import 'package:flutter/material.dart';
import 'package:frontend/Components/opportunity_card.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:number_paginator/number_paginator.dart';

class PaginatedOpportunityGallery extends StatefulWidget {
  final List<Opportunity> allOpportunities;

  const PaginatedOpportunityGallery({super.key, required this.allOpportunities});

  @override
  State<PaginatedOpportunityGallery> createState() =>
      _PaginatedOpportunityGalleryState();
}

class _PaginatedOpportunityGalleryState
    extends State<PaginatedOpportunityGallery> {
  final int _numPages = 10;
  int _currentPage = 0;

  static const int itemsPerPage = 6;

  List<Opportunity> get _currentPageOpportunities {
    final startIndex = _currentPage * itemsPerPage;
    final endIndex =
        (startIndex + itemsPerPage).clamp(0, widget.allOpportunities.length);
    return widget.allOpportunities.sublist(startIndex, endIndex);
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Wrapping the GridView in a SingleChildScrollView allows for scrolling
        // while maintaining the flexibility of pagination
        SingleChildScrollView(
          child: Column(
            children: [
              // GridView with shrinkWrap to limit the amount of space it takes
              GridView.builder(
                shrinkWrap: true, // Makes the grid take only the required space
                physics: NeverScrollableScrollPhysics(), // Disable scrolling on the GridView itself
                gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(
                  maxCrossAxisExtent: 400,
                  crossAxisSpacing: 8.0,
                  mainAxisSpacing: 8.0,
                ),
                itemCount: _currentPageOpportunities.length,
                itemBuilder: (context, index) {
                  return Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: OpportunityCard(
                      opportunity: _currentPageOpportunities[index],
                    ),
                  );
                },
              ),
              // Pagination control
              Padding(
                padding: const EdgeInsets.all(8.0),
                child: NumberPaginator(
                  numberPages: _numPages,
                  onPageChange: (int index) {
                    setState(() {
                      _currentPage = index;
                    });
                  },
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
