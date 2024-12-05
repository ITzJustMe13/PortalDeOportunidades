import 'package:flutter/material.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityImages.dart';
import 'package:frontend/Components/OpportunityLocationMap.dart';
import 'package:frontend/Components/OpportunityDetails.dart';
import 'package:frontend/Components/OpportunityAdditionalInfo.dart';
import 'package:frontend/Components/ReservationButton.dart';
import 'package:frontend/State/DetailsState.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:intl/intl.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Components/ReviewCard.dart';
import 'package:frontend/Components/StarRating.dart';

class OpportunityDetailsScreen extends StatefulWidget {
  final bool isReservable;
  final Opportunity opportunity;

  const OpportunityDetailsScreen(
      {super.key, required this.opportunity, this.isReservable = true});

  @override
  _OpportunityDetailScreenState createState() =>
      _OpportunityDetailScreenState();
}

class _OpportunityDetailScreenState extends State<OpportunityDetailsScreen> {
  final ScrollController verticalScrollController = ScrollController();
  final ScrollController horizontalScrollController = ScrollController();

  String formattedTime = "";
  bool _isInitialized = false;

  @override
  void initState() {
    super.initState();
    // Perform initialization here
    final DateTime dateTime = widget.opportunity.date;
    formattedTime = DateFormat('HH:mm').format(dateTime);

    // Use WidgetsBinding to access the context after the widget is built
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final detailsState = Provider.of<DetailsState>(context, listen: false);
      if (!_isInitialized) {
        detailsState.initialize(widget.opportunity);
        _isInitialized = true;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: Consumer<DetailsState>(
        builder: (context, detailsState, child) {
          return LayoutBuilder(
            builder: (context, constraints) {
              if (constraints.maxWidth < 600) {
                return _buildMobileLayout(widget.opportunity, formattedTime,
                    detailsState.owner, detailsState);
              } else if (constraints.maxWidth < 1200) {
                return _buildTabletLayout(widget.opportunity, formattedTime,
                    detailsState.owner, detailsState);
              } else {
                return _buildDesktopLayout(widget.opportunity, formattedTime,
                    detailsState.owner, detailsState);
              }
            },
          );
        },
      ),
    );
  }

  // Mobile layout (Vertical scroll)
  Widget _buildMobileLayout(Opportunity opportunity, String time, User? user,
      DetailsState detailsState) {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          OpportunityImages(opportunity: opportunity),
          SizedBox(height: 20),
          StarRating(rating: opportunity.reviewScore),
          SizedBox(height: 10),
          OpportunityDetails(
            name: opportunity.name,
            description: opportunity.description,
          ),
          SizedBox(height: 20),
          DynamicActionButton(
            text: "Adicionar aos Favoritos",
            color: Color(0xFF50C878),
            icon: Icons.star,
            onPressed: () {
              detailsState.addToFavorites(
                  context, widget.opportunity.opportunityId);
            },
          ),
          SizedBox(height: 20),
          if (widget.isReservable &&
              !detailsState.isOwner &&
              widget.opportunity.isActive &&
              detailsState.priceWithTax != null)
            ReservationButton(
              availableVacancies: 2,
              onPressed: (numberOfPersons) {
                detailsState.createTempReservation(
                    context, numberOfPersons, opportunity);
              },
            ),
          SizedBox(height: 20),
          OpportunityAdditionalInfo(
            price: detailsState.priceWithTax,
            location: opportunity.location,
            address: opportunity.address,
            vacancies: opportunity.vacancies,
            firstName: user?.firstName ?? "Anónimo",
            lastName: user?.lastName ?? "Anónimo",
            time: time,
            date: opportunity.date,
          ),
          SizedBox(height: 20),
          OpportunityLocationMap(
            address: opportunity.address,
          ),
          SizedBox(height: 20),
          Text(
            'Opiniões',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          // Check if reviews are loading
          detailsState.isLoading
              ? Center(child: CircularProgressIndicator())
              : detailsState.reviews.isEmpty
                  ? Text('Nenhuma opinião disponível.')
                  : Column(
                      children: detailsState.reviews.map((review) {
                        return ReviewCard(
                          rating: review.rating,
                          description: review.description ?? '',
                        );
                      }).toList(),
                    ),
        ],
      ),
    );
  }

  // Tablet layout (Vertical scroll with Scrollbar)
  Widget _buildTabletLayout(Opportunity opportunity, String time, User? user,
      DetailsState detailsState) {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left Column (Image and Details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityImages(opportunity: opportunity),
                  StarRating(rating: opportunity.reviewScore),
                  OpportunityDetails(
                    name: opportunity.name,
                    description: opportunity.description,
                  ),
                  SizedBox(height: 20),
                  DynamicActionButton(
                    text: "Adicionar aos Favoritos",
                    color: Color(0xFF50C878),
                    icon: Icons.star,
                    onPressed: () {
                      detailsState.addToFavorites(
                          context, widget.opportunity.opportunityId);
                    },
                  ),
                  if (widget.isReservable &&
                      !detailsState.isOwner &&
                      widget.opportunity.isActive)
                    ReservationButton(
                      availableVacancies: opportunity.vacancies,
                      onPressed: (numberOfPersons) {
                        detailsState.createTempReservation(
                            context, numberOfPersons, opportunity);
                      },
                    ),
                ],
              ),
            ),
            SizedBox(width: 20), // Add space between the two columns

            // Right Column (Additional Info, Map, and Reviews)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: detailsState.priceWithTax,
                    location: opportunity.location,
                    address: opportunity.address,
                    vacancies: opportunity.vacancies,
                    firstName: user?.firstName ?? "Anónimo",
                    lastName: user?.lastName ?? "Anónimo",
                    time: time,
                    date: opportunity.date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(
                    address: opportunity.address,
                  ),
                  // REVIEWS
                  SizedBox(height: 20),
                  Text(
                    'Opiniões',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  // Check if reviews are loading
                  detailsState.isLoading
                      ? Center(child: CircularProgressIndicator())
                      : detailsState.reviews.isEmpty
                          ? Text('Nenhuma opinião disponível.')
                          : Column(
                              children: detailsState.reviews.map((review) {
                                return ReviewCard(
                                  rating: review.rating,
                                  description: review.description ?? '',
                                );
                              }).toList(),
                            ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Desktop layout (Both vertical and horizontal scroll with Scrollbar)
  Widget _buildDesktopLayout(Opportunity opportunity, String time, User? user,
      DetailsState detailsState) {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        scrollDirection: Axis.vertical,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left column (images and details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SizedBox(height: 20),
                  OpportunityImages(opportunity: opportunity),
                  StarRating(rating: opportunity.reviewScore),
                  OpportunityDetails(
                    name: opportunity.name,
                    description: opportunity.description,
                  ),
                  SizedBox(height: 20),
                  DynamicActionButton(
                    text: "Adicionar aos Favoritos",
                    color: Color(0xFF50C878),
                    icon: Icons.star,
                    onPressed: () {
                      detailsState.addToFavorites(
                          context, widget.opportunity.opportunityId);
                    },
                  ),
                  SizedBox(height: 20),
                  if (widget.isReservable &&
                      !detailsState.isOwner &&
                      widget.opportunity.isActive)
                    ReservationButton(
                      availableVacancies: widget.opportunity.vacancies,
                      onPressed: (numberOfPersons) {
                        detailsState.createTempReservation(
                            context, numberOfPersons, opportunity);
                      },
                    ),
                ],
              ),
            ),
            SizedBox(width: 20),
            // Right column (additional info, map, and reviews)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: detailsState.priceWithTax,
                    location: opportunity.location,
                    address: opportunity.address,
                    vacancies: opportunity.vacancies,
                    firstName: user?.firstName ?? "Anónimo",
                    lastName: user?.lastName ?? "Anónimo",
                    time: time,
                    date: opportunity.date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(
                    address: opportunity.address,
                  ),
                  SizedBox(height: 20),
                  Text(
                    'Opiniões',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  detailsState.isLoading
                      ? Center(child: CircularProgressIndicator())
                      : detailsState.reviews.isEmpty
                          ? Text('Nenhuma opinião disponível.')
                          : Column(
                              children: detailsState.reviews.map((review) {
                                return ReviewCard(
                                  rating: review.rating,
                                  description: review.description ?? '',
                                );
                              }).toList(),
                            ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
