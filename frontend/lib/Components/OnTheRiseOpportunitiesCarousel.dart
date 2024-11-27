import 'package:carousel_slider/carousel_slider.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/OnTheRiseOpportunityCard.dart';
import 'package:frontend/Models/Opportunity.dart';

class OnTheRiseOpportunityCarousel extends StatelessWidget {
  final List<Opportunity> opportunitiesOnTheRiseList;

  const OnTheRiseOpportunityCarousel(
      {super.key, required this.opportunitiesOnTheRiseList});

  @override
  Widget build(BuildContext context) {
    final double screenWidth = MediaQuery.of(context).size.width;
    final bool isPhone = screenWidth < 600;

    return CarouselSlider(
      options: CarouselOptions(
        height: isPhone ? 300 : 400,
        viewportFraction: isPhone ? 0.9 : 0.8,
        enlargeFactor: 0.125,
        enlargeCenterPage: true,
        enableInfiniteScroll: true,
        autoPlay: true,
      ),
      items: opportunitiesOnTheRiseList
          .chunked(isPhone
              ? 1
              : 3) // Group items into chunks of 3 for non-phone devices
          .map((chunkedOpportunities) {
        return Column(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: chunkedOpportunities.map((opportunity) {
            return Expanded(
              child: Padding(
                padding: const EdgeInsets.all(8.0),
                child: OnTheRiseOpportunityCard(
                  opportunity: opportunity,
                ),
              ),
            );
          }).toList(),
        );
      }).toList(),
    );
  }
}

extension ListExtensions<T> on List<T> {
  /// Breaks a list into chunks of the given size
  List<List<T>> chunked(int chunkSize) {
    List<List<T>> chunks = [];
    for (var i = 0; i < length; i += chunkSize) {
      chunks.add(sublist(i, i + chunkSize > length ? length : i + chunkSize));
    }
    return chunks;
  }
}
