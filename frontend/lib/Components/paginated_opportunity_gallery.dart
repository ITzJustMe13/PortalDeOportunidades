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
  int _currentPage = 0;

  static const int itemsPerPage = 6;

  // Calcular o total de páginas dinamicamente
  int get _numPages {
    final totalItems = widget.allOpportunities.length;
    return (totalItems / itemsPerPage).ceil();
  }

  List<Opportunity> get _currentPageOpportunities {
    final startIndex = _currentPage * itemsPerPage;
    final endIndex = (startIndex + itemsPerPage).clamp(0, widget.allOpportunities.length);
    return widget.allOpportunities.sublist(startIndex, endIndex);
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SingleChildScrollView(
          child: Column(
            children: [
              GridView.builder(
                shrinkWrap: true, 
                physics: NeverScrollableScrollPhysics(), 
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
                  numberPages: _numPages,  // Now it dynamically reflects the number of pages
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
