import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Models/ReviewOportunityReservation.dart';
import 'package:frontend/State/ReviewHistoryState.dart';
import 'package:provider/provider.dart';

class ReviewsHistoryScreen extends StatelessWidget {
  const ReviewsHistoryScreen({super.key});

  static const int itemsPerPage = 16;

  @override
  Widget build(BuildContext context) {
    return Consumer<ReviewHistoryState>(
        builder: (context, reviewHistoryState, child) {
      return Scaffold(
        appBar: CustomAppBar(),
        endDrawer: CustomDrawer(),
        body: LayoutBuilder(
          builder: (context, constraints) {
            if (constraints.maxWidth < 600) {
              return _buildMobileLayout(reviewHistoryState);
            } else if (constraints.maxWidth < 1200) {
              return _buildTabletLayout(reviewHistoryState);
            } else {
              return _buildDesktopLayout(reviewHistoryState);
            }
          },
        ),
      );
    });
  }

  Widget _buildMobileLayout(reviewHistoryState) {
    return _buildReviewsList(reviewHistoryState);
  }

  Widget _buildTabletLayout(reviewHistoryState) {
    return _buildReviewsList(reviewHistoryState);
  }

  Widget _buildDesktopLayout(reviewHistoryState) {
    return _buildReviewsList(reviewHistoryState);
  }

  Widget _buildReviewsList(reviewHistoryState) {
    int startIndex = (reviewHistoryState.currentPage - 1) * itemsPerPage;
    int endIndex =
        (startIndex + itemsPerPage) > reviewHistoryState.reviewList.length
            ? reviewHistoryState.reviewList.length
            : (startIndex + itemsPerPage);

    List<ReviewOpportunityReservation> paginatedReviews =
        reviewHistoryState.reviewList.sublist(startIndex, endIndex);

    return Column(
      children: [
        Expanded(
          child: ListView.builder(
            padding: const EdgeInsets.all(16.0),
            itemCount: paginatedReviews.length,
            itemBuilder: (context, index) {
              Review review = paginatedReviews[index].review;
              Reservation reservation = paginatedReviews[index].reservation;
              Opportunity opportunity = paginatedReviews[index].opportunity;
              return _buildReviewCard(review, reservation, opportunity);
            },
          ),
        ),
        _buildPaginationControls(reviewHistoryState),
      ],
    );
  }

  Widget _buildPaginationControls(reviewHistoryState) {
    int totalPages =
        (reviewHistoryState.reviewList.length / itemsPerPage).ceil();

    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.arrow_back),
            onPressed: reviewHistoryState.currentPage > 1
                ? () {
                    reviewHistoryState.currentPage--;
                  }
                : null,
          ),
          Text(
            'Page ${reviewHistoryState.currentPage} of $totalPages',
            style: const TextStyle(fontSize: 16),
          ),
          IconButton(
            icon: const Icon(Icons.arrow_forward),
            onPressed: reviewHistoryState.currentPage < totalPages
                ? () {
                    reviewHistoryState.currentPage++;
                  }
                : null,
          ),
        ],
      ),
    );
  }

  Widget _buildReviewCard(
      Review review, Reservation reservation, Opportunity opportunity) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16.0),
      elevation: 4,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12.0),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildOpportunityName(opportunity),
            const SizedBox(height: 8),
            _buildReservationDate(reservation),
            const SizedBox(height: 8),
            _buildRating(review),
            const SizedBox(height: 8),
            _buildDescription(review),
          ],
        ),
      ),
    );
  }

  Widget _buildOpportunityName(Opportunity opportunity) {
    return Text(
      opportunity.name,
      style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
    );
  }

  Widget _buildReservationDate(Reservation reservation) {
    return Text(
      "Data Reserva: ${reservation.reservationDate}",
      style: const TextStyle(fontSize: 14, color: Colors.grey),
    );
  }

  Widget _buildRating(Review review) {
    return Row(
      children: [
        const Text(
          "Rating: ",
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
        ),
        Text(
          "${review.rating}/5",
          style: const TextStyle(fontSize: 16),
        ),
      ],
    );
  }

  Widget _buildDescription(Review review) {
    return Text(
      review.description ?? "Nenhuma descrição disponível",
      style: const TextStyle(fontSize: 14),
    );
  }
}
