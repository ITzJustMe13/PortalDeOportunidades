import 'package:flutter/material.dart';
import 'package:frontend/Components/opportunity_card.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:number_paginator/number_paginator.dart';

class PaginatedOpportunityGallery extends StatefulWidget {
  final List<Opportunity> allOpportunities;

  const PaginatedOpportunityGallery(
      {super.key, required this.allOpportunities});

  @override
  State<PaginatedOpportunityGallery> createState() =>
      _PaginatedOpportunityGalleryState();
}

class _PaginatedOpportunityGalleryState
    extends State<PaginatedOpportunityGallery> {
  int _currentPage = 0;

  int _itemsPerPage = 10; // Default items per page

  List<int> _itemsPerPageOptions = [5, 10, 15, 20, 25];

  // Calcular o total de páginas dinamicamente
  int get _numPages {
    final totalItems = widget.allOpportunities.length;
    return (totalItems / _itemsPerPage).ceil();
  }

  List<Opportunity> get _currentPageOpportunities {
    final startIndex = _currentPage * _itemsPerPage;
    final endIndex =
        (startIndex + _itemsPerPage).clamp(0, widget.allOpportunities.length);
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
                  numberPages:
                      _numPages, // Now it dynamically reflects the number of pages
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
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: Row(
            children: [
              Text('Items per page:'),
              SizedBox(width: 10),
              DropdownButton<int>(
                value: _itemsPerPage,
                onChanged: (newValue) {
                  setState(() {
                    _itemsPerPage = newValue!;
                    _currentPage =
                        0; // Reset to first page when items per page changes
                  });
                },
                items: _itemsPerPageOptions.map((int value) {
                  return DropdownMenuItem<int>(
                    value: value,
                    child: Text(value.toString()),
                  );
                }).toList(),
              ),
            ],
          ),
        ),
      ],
    );
  }
}